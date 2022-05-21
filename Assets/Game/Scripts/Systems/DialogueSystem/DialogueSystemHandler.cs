using Game.Systems.ContextMenu;
using Game.Systems.DialogueSystem.Nodes;

using I2.Loc;

using NodeCanvas.DialogueTrees;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using DG.Tweening;

using Zenject;
using UnityEngine.UIElements;
using Game.Systems.SheetSystem;

namespace Game.Systems.DialogueSystem
{
	public class DialogueSystemHandler : IInitializable, IDisposable
	{
		public bool IsDialogueProcess => dialogueCoroutine != null;
		private Coroutine dialogueCoroutine = null;

		private bool isWaitingChoice;
		private MultipleChoiceRequestInfo cachedChoiceInfo;

		private UIDialogue dialogue;
		private List<UISubtitle> subtitles = new List<UISubtitle>();
		private List<UINotification> notifications = new List<UINotification>();
		private List<ChoiceWrapper> choices = new List<ChoiceWrapper>();

		private Settings settings;
		private SignalBus signalBus;
		private UIManager uiManager;
		private AsyncManager asyncManager;
		private UISubtitle.Factory subtitleFactory;
		private UINotification.Factory notificationFactory;
		private UIChoice.Factory choiceFactory;

		public DialogueSystemHandler(Settings settings, SignalBus signalBus, UIManager uiManager, AsyncManager asyncManager,
			UISubtitle.Factory subtitleFactory, UINotification.Factory notificationFactory, UIChoice.Factory choiceFactory)
		{
			this.settings = settings;
			this.signalBus = signalBus;
			this.uiManager = uiManager;
			this.asyncManager = asyncManager;
			this.subtitleFactory = subtitleFactory;
			this.notificationFactory = notificationFactory;
			this.choiceFactory = choiceFactory;

			dialogue = uiManager.Dialogue;
		}

		public void Initialize()
		{
			dialogue.Enable(false);
			dialogue.SubtitlesContent.DestroyChildren();
			dialogue.NotificationContent.DestroyChildren();
			dialogue.ChoiceContent.DestroyChildren();

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


		private IEnumerator CalculateHeight()
		{
			//Canvas.ForceUpdateCanvases();

			yield return null;

			#region Resize Spacer
			Assert.AreNotEqual(dialogue.SubtitlesContent.childCount, 0, "dialogue.SubtitlesContent.childCount == 0");

			var currentSubtitle = dialogue.SubtitlesContent.GetChild(dialogue.SubtitlesContent.childCount - 1) as RectTransform;
			float currentSubtitleHeight = LayoutUtility.GetPreferredHeight(currentSubtitle);
			float choiceHeight = LayoutUtility.GetPreferredHeight(dialogue.ChoiceContent as RectTransform);
			float contentSpaces = dialogue.ContentLayoutGroup.spacing * 2;
			float subtitlesSpacing = dialogue.SubtitlesLayoutGroup.spacing;

			float height = (dialogue.RectTransform.sizeDelta.y) - (currentSubtitleHeight + choiceHeight + (contentSpaces + subtitlesSpacing));//maxHeight - currentHeight
			dialogue.Spacer.preferredHeight = height <= 0 ? 0 : height;
			#endregion

			yield return null;

			#region Scroll To Element Top Position
			var topPosition = currentSubtitle.position;
			topPosition.y += (currentSubtitle.sizeDelta.y / 2) + subtitlesSpacing;

			var destination = (Vector2)dialogue.Content.transform.InverseTransformPoint(dialogue.Content.position) - (Vector2)dialogue.Content.transform.InverseTransformPoint(topPosition);
			destination.x = 0;

			DOTween.To(() => dialogue.Content.anchoredPosition, x => dialogue.Content.anchoredPosition = x, destination, 0.25f);
			#endregion
		}


		private void SpawnSubtitles(IActor actor, string text)
		{
			var sheet = actor.Sheet;
			dialogue.ActorPortrait.gameObject.SetActive(sheet.Information.IsHasPortrait);
			dialogue.ActorPortrait.sprite = sheet.Information.portrait;

			var subtitle = subtitleFactory.Create();

			subtitle.Text.text = $"{sheet.Information.Name} - {text}";

			subtitle.transform.SetParent(dialogue.SubtitlesContent);

			subtitles.Add(subtitle);

			asyncManager.StartCoroutine(CalculateHeight());
		}
		private void DispawnSubtitles()
		{
			for (int i = subtitles.Count - 1; i >= 0; i--)
			{
				subtitles[i].DespawnIt();
				subtitles.Remove(subtitles[i]);
			}
		}
		private IEnumerator Subtitles(SubtitlesRequestInfo info)
		{
			var audio = info.statement.Audio;
			var actor = info.actor as IActor;

			Assert.IsNotNull(actor, "IActor == null");

			SpawnSubtitles(actor, info.statement.Text);

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

		private void SpawnNotifications(ChoiceWrapper choiceWrapper)
		{
			choiceWrapper.notifications.ForEach((x) =>
			{
				var notification = notificationFactory.Create();

				notification.Text.text = x.text;
				notification.Text.color = x.textColor;

				notification.transform.SetParent(dialogue.NotificationContent);

				notifications.Add(notification);
			});
		}
		private void DispawnNotifications()
		{
			for (int i = notifications.Count - 1; i >= 0; i--)
			{
				notifications[i].DespawnIt();
				notifications.Remove(notifications[i]);
			}
		}

		private void DespawnChoices()
		{
			for (int i = choices.Count - 1; i >= 0; i--)
			{
				choices[i].ui.DespawnIt();
				choices[i].ui.onButtonClick -= OnChoiced;
				choices.Remove(choices[i]);
			}
		}


		private void OnDialogueStarted(DialogueTree tree)
		{
			dialogue.Enable(true);
		}

		private void OnDialoguePaused(DialogueTree tree) { }

		private void OnDialogueFinished(DialogueTree tree)
		{
			dialogue.Enable(false);
			DispawnSubtitles();
			DispawnNotifications();
			dialogue.Spacer.preferredHeight = 0;
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

			DespawnChoices();

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

				choices.Add(wrapper);
				choiceUI.onButtonClick += OnChoiced;
				choiceUI.transform.SetParent(dialogue.ChoiceContent);
			}

			asyncManager.StartCoroutine(CalculateHeight());
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
					wrapper.ui.Text.text = $"{index}. {GetConsequence()}{GetRequirements(true)}";
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

				return string.IsNullOrEmpty(requires) ? "" : $"[Requires {requires}] " + (includeLabel ? errors : "");
			}
			string GetConsequence()
			{
				string consequence = "";

				wrapper.choice.consequence.ForEach((x) =>
				{
					if (x is CommandSetAlignment commandAlignment)
					{
						consequence += commandAlignment.Target;

						wrapper.notifications.Add(new Notification() { text = $"You've performed a {commandAlignment.Target} action", textColor = AlignmentCharacteristic.GetAlignmentColor(commandAlignment.Target) });
						if(commandAlignment.Current != commandAlignment.Forecast)
						{
							wrapper.notifications.Add(new Notification() { text = $"You new alignment is {commandAlignment.Forecast}", textColor = AlignmentCharacteristic.GetAlignmentColor(commandAlignment.Forecast) });
						}
					}
				});
				consequence = !string.IsNullOrEmpty(consequence) ? $"({consequence}) " : "";//spaces

				return consequence;
			}
		}


		private void OnChoiced(UIChoice choice)
		{
			var wrapper = choices.Find((x) => x.ui == choice);

			if(wrapper.choice.choiceConditionState == ChoiceConditionState.Normal)
			{
				if (settings.isSayChoice)
				{
					var actor = cachedChoiceInfo.actor as IActor;
					Assert.IsNotNull(actor, "MultipleChoiceRequestInfo IActor == null");
					SpawnSubtitles(actor, wrapper.option.Statement.Text);

					SpawnNotifications(wrapper);
				}

				int index = choices.IndexOf(wrapper);
				DespawnChoices();
				cachedChoiceInfo.SelectOption(index);

				isWaitingChoice = false;
			}
		}

		public class ChoiceWrapper
		{
			public Choice choice;
			public ChoiceOption option;
			public UIChoice ui;

			public List<Notification> notifications = new List<Notification>();
		}


		[System.Serializable]
		public class Settings
		{
			public bool isSayChoice = true;

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

	public class Notification
	{
		public string text;
		public Color textColor = Color.white;
	}
}