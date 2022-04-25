using FlowCanvas.Nodes;

using NodeCanvas.DialogueTrees;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

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
		private List<UIChoice> choices = new List<UIChoice>();

		private Settings settings;
		private UIManager uiManager;
		private AsyncManager asyncManager;
		private UIChoice.Factory choiceFactory;

		public DialogueSystemHandler(Settings settings, UIManager uiManager, AsyncManager asyncManager, UIChoice.Factory choiceFactory)
		{
			this.settings = settings;
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


		IEnumerator Internal_OnSubtitlesRequestInfo(SubtitlesRequestInfo info)
		{
			var text = info.statement.text;
			var audio = info.statement.audio;
			var actor = info.actor;

			//actorSpeech.color = ;

			//actorPortrait.gameObject.SetActive(actor.portraitSprite != null);
			//actorPortrait.sprite = actor.portraitSprite;

			if (settings.additionalText)
			{
				//dialogue.ActorSpeech.text += $"<color=#{ColorUtility.ToHtmlStringRGBA(actor.ActorColor)}>{actor.ActorName}</color> - {text}\n";
			}
			else
			{
				dialogue.ActorSpeech.text = text;
			}


			if (settings.waitForInput)
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

		void OnDialogueStarted(DialogueTree tree)
		{
			dialogue.ActorSpeech.text = "";
			dialogue.Enable(true);
		}

		void OnDialoguePaused(DialogueTree tree) { }

		void OnDialogueFinished(DialogueTree tree)
		{
			dialogue.Enable(false);
		}

		void OnSubtitlesRequest(SubtitlesRequestInfo info)
		{
			if (!IsDialogueProcess)
			{
				dialogueCoroutine = asyncManager.StartCoroutine(Internal_OnSubtitlesRequestInfo(info));
			}
		}

		void OnMultipleChoiceRequest(MultipleChoiceRequestInfo info)
		{
			cachedChoiceInfo = info;

			//clear
			for (int i = choices.Count - 1; i >= 0; i--)
			{
				choices[i].DespawnIt();
				choices[i].onButtonClick -= OnChoiced;
				choices.Remove(choices[i]);
			}

			int index = 0;
			foreach (KeyValuePair<IStatement, int> pair in info.options)
			{
				UIChoice choice = choiceFactory.Create();
				choice.Text.text = $"[{index+1}] {pair.Value} {pair.Key.text}";
				choice.onButtonClick += OnChoiced;
				choice.transform.SetParent(dialogue.ChoiceContent);

				choices.Add(choice);

				index++;
			}
		}

		void OnChoiced(UIChoice choice)
		{
			int index = choices.IndexOf(choice);

			isWaitingChoice = false;
			//clear
			for (int i = choices.Count - 1; i >= 0; i--)
			{
				choices[i].DespawnIt();
				choices[i].onButtonClick -= OnChoiced;
				choices.Remove(choices[i]);
			}
			cachedChoiceInfo.SelectOption(index);
		}


		[System.Serializable]
		public class Settings
		{
			public bool additionalText = true;
			public bool skipOnInput;
			public bool waitForInput = true;

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