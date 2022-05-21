
using Game.Entities;
using Game.Systems.ContextMenu;
using Game.Systems.DialogueSystem.Nodes;
using Game.Systems.SheetSystem;

using I2.Loc;

using NodeCanvas.DialogueTrees;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.Systems.DialogueSystem
{
	public class DialogueSystemHandler : IInitializable, IDisposable
	{
		public bool IsDialogueProcess => dialogueCoroutine != null;
		private Coroutine dialogueCoroutine = null;

		private bool isWaitingChoice;
		private MultipleChoiceRequestInfo cachedChoiceInfo;

		private UIDialogue dialogue;
		private List<ChoiceWrapper> choices = new List<ChoiceWrapper>();

		private Settings settings;
		private SignalBus signalBus;
		private UIManager uiManager;
		private AsyncManager asyncManager;
		private UIChoice.Factory choiceFactory;

		public DialogueSystemHandler(Settings settings, SignalBus signalBus, UIManager uiManager, AsyncManager asyncManager, UIChoice.Factory choiceFactory)
		{
			this.settings = settings;
			this.signalBus = signalBus;
			this.uiManager = uiManager;
			this.asyncManager = asyncManager;
			this.choiceFactory = choiceFactory;

			dialogue = uiManager.Dialogue;
		}

		public void Initialize()
		{
			dialogue.EnableContinueButton(false);
			dialogue.ChoiceContent.DestroyChildren();
			dialogue.Enable(false);

			DialogueTree.OnDialogueStarted += OnDialogueStarted;
			DialogueTree.OnDialoguePaused += OnDialoguePaused;
			DialogueTree.OnDialogueFinished += OnDialogueFinished;
			DialogueTree.OnSubtitlesRequest += OnSubtitlesRequest;
			DialogueTree.OnMultipleChoiceRequest += OnMultipleChoiceRequest;
		}

		public void Dispose()
		{
			DialogueTree.OnDialogueStarted -= OnDialogueStarted;
			DialogueTree.OnDialoguePaused -= OnDialoguePaused;
			DialogueTree.OnDialogueFinished -= OnDialogueFinished;
			DialogueTree.OnSubtitlesRequest -= OnSubtitlesRequest;
			DialogueTree.OnMultipleChoiceRequest -= OnMultipleChoiceRequest;
		}


		private IEnumerator Subtitles(SubtitlesRequestInfo info)
		{
			var text = info.statement.Text;
			var audio = info.statement.Audio;
			var actor = info.actor as IActor;

			Assert.IsNotNull(actor, "IActor == null");

			var sheet = (actor as ISheetable).Sheet;
			dialogue.ActorPortrait.gameObject.SetActive(sheet.Information.IsHasPortrait);
			dialogue.ActorPortrait.sprite = sheet.Information.portrait;

			if (settings.additionalText)
			{
				dialogue.ActorSpeech.text += $"{sheet.Information.Name} - {text}\n";
			}
			else
			{
				dialogue.ActorSpeech.text = $"{sheet.Information.Name} - {text}\n";
			}

			//Wait For Input
			if (info.waitForInput)
			{
				dialogue.WaitIndicator.SetActive(true);
				while (!Input.anyKeyDown)
				{
					yield return null;
				}
				dialogue.WaitIndicator.SetActive(false);
			} 

			yield return null;

			dialogueCoroutine = null;

			info.Continue();
		}

		private void OnDialogueStarted(DialogueTree tree)
		{
			dialogue.ActorSpeech.text = "";
			dialogue.Enable(true);
		}

		private void OnDialoguePaused(DialogueTree tree) { }

		private void OnDialogueFinished(DialogueTree tree)
		{
			dialogue.Enable(false);
			dialogue.ActorSpeech.text = "";
		}

		private void OnSubtitlesRequest(SubtitlesRequestInfo info)
		{
			if (!IsDialogueProcess)
			{
				dialogueCoroutine = asyncManager.StartCoroutine(Subtitles(info));
			}
		}

		private void OnMultipleChoiceRequest(MultipleChoiceRequestInfo info)
		{
			cachedChoiceInfo = info;

			//clear
			for (int i = choices.Count - 1; i >= 0; i--)
			{
				choices[i].ui.DespawnIt();
				choices[i].ui.onButtonClick -= OnChoiced;
				choices.Remove(choices[i]);
			}


			int langIndex = LocalizationManager.GetAllLanguages(true).IndexOf(LocalizationManager.CurrentLanguage);

			//fill
			for (int i = 0; i < info.choices.Count; i++)
			{
				Choice choice = info.choices[i];
				
				if (!(langIndex >= 0 && langIndex < choice.options.Count))
				{
					throw new Exception("LocalizationManager index out of bounds!");
				}

				UIChoice choiceUI = choiceFactory.Create();

				ChoiceWrapper wrapper = new ChoiceWrapper()
				{
					choice = choice,
					option = choice.options[langIndex],
					ui = choiceUI
				};

				FillChoice(i + 1, wrapper);

				choiceUI.onButtonClick += OnChoiced;
				choiceUI.transform.SetParent(dialogue.ChoiceContent);

				choices.Add(wrapper);
			}
		}

		private void FillChoice(int index, ChoiceWrapper wrapper)
		{
			switch (wrapper.choice.choiceConditionState)
			{
				case ChoiceConditionState.Normal:
				{
					wrapper.ui.Text.color = wrapper.choice.isSelected ? Color.gray : Color.white;
					wrapper.ui.Text.text = $"{index}. {GetConsequence()}{GetRequirements()}{wrapper.option.Statement.Text}";//1. (Aligment or Lie-True) [Requirements or Action] Actor - Text.
					break;
				}
				case ChoiceConditionState.Inactive:
				{
					wrapper.ui.Text.color = new Color(0.35f, 0.35f, 0.35f);//dark gray
					wrapper.ui.Text.text = $"{index}. {GetConsequence()}[Conditions Not Met] {wrapper.option.Statement.Text}";
					break;
				}
				case ChoiceConditionState.Unavailable:
				{
					wrapper.ui.Text.color = Color.red;
					wrapper.ui.Text.text = $"{index}. [Unavailable Choice]";
					break;
				}
				case ChoiceConditionState.Reason:
				{
					wrapper.ui.Text.color = Color.red;
					wrapper.ui.Text.text = $"{index}. {GetConsequence()} {GetRequirements(true)}";
					break;
				}
			}


			string GetRequirements(bool includeLabel = false)
			{
				string requires = "";
				string errors = "";

				wrapper.choice.requirements.ForEach((x) =>
				{
					if (x is AlignmentRequirement alignmentRequirement)
					{
						requires += alignmentRequirement.alignmentRequired;
						errors += "Specific alignment required";
					}
				});
				requires = !string.IsNullOrEmpty(requires) ? $"{requires} " : "";//spaces

				return string.IsNullOrEmpty(requires) ? "" : $"[Requires {requires}] " + (includeLabel ? errors : "");
			}
			string GetConsequence()
			{
				string consequence = "";

				wrapper.choice.consequence.ForEach((x) =>
				{
					if (x is CommandSetAlignment commandAlignment)
					{
						consequence += commandAlignment.Result;
					}
				});
				consequence = !string.IsNullOrEmpty(consequence) ? $"{consequence} " : "";//spaces

				return consequence;
			}
		}

		private void OnChoiced(UIChoice choice)
		{
			var wrapper = choices.Find((x) => x.ui == choice);

			if(wrapper.choice.choiceConditionState == ChoiceConditionState.Normal)
			{
				isWaitingChoice = false;

				int index = choices.IndexOf(wrapper);

				//clear
				for (int i = choices.Count - 1; i >= 0; i--)
				{
					choices[i].ui.DespawnIt();
					choices[i].ui.onButtonClick -= OnChoiced;
					choices.Remove(choices[i]);
				}
				cachedChoiceInfo.SelectOption(index);
			}
		}

		public class ChoiceWrapper
		{
			public Choice choice;
			public ChoiceOption option;
			public UIChoice ui;
		}


		[System.Serializable]
		public class Settings
		{
			public bool additionalText = true;
			public bool skipOnInput;

			public SubtitleDelays delays;

			[System.Serializable]
			public class SubtitleDelays
			{
				public float characterDelay = 0.05f;
				public float sentenceDelay = 0.5f;
				public float commaDelay = 0.1f;
				public float finalDelay = 1.2f;
			}
		}
	}
}