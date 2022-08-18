using DG.Tweening;

using Game.Entities.Models;
using Game.Systems.CommandCenter;
using Game.Systems.ContextMenu;
using Game.Systems.DialogueSystem.Nodes;
using Game.Systems.SheetSystem;
using Game.UI;
using Game.UI.Windows;

using NodeCanvas.DialogueTrees;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.DialogueSystem
{
	public class DialogueSystemWindow : WindowBase
	{
		public RectTransform RectTransform => transform as RectTransform;

		[field: SerializeField] public LayoutElement Spacer { get; private set; }
		[field: SerializeField] public Button Button { get; private set; }
		[field: Space]
		[field: SerializeField] public Image ActorPortrait { get; private set; }
		[field: SerializeField] public GameObject WaitIndicator { get; private set; }
		[field: Space]
		[field: SerializeField] public RectTransform Content { get; private set; }
		[field: SerializeField] public Transform SubtitlesContent { get; private set; }
		[field: SerializeField] public Transform NotificationContent { get; private set; }
		[field: SerializeField] public Transform ChoiceContent { get; private set; }
		[field: Space]
		[field: SerializeField] public VerticalLayoutGroup ContentLayoutGroup { get; private set; }
		[field: SerializeField] public VerticalLayoutGroup SubtitlesLayoutGroup { get; private set; }

		private bool isWaitInput = false;
		private bool isWaitChoiceInput = false;

		private List<UISubtitle> subtitles = new List<UISubtitle>();
		private List<UINotification> notifications = new List<UINotification>();
		private List<ChoiceWrapper> choices = new List<ChoiceWrapper>();

		private MultipleChoiceRequestInfo cachedChoiceInfo;


		private Settings settings;
		private UISubCanvas subCanvas;
		private AsyncManager asyncManager;
		private UISubtitle.Factory subtitleFactory;
		private UINotification.Factory notificationFactory;
		private UIChoice.Factory choiceFactory;

		[Inject]
		private void Construct(
			Settings settings,
			UISubCanvas subCanvas,
			AsyncManager asyncManager,
			UISubtitle.Factory subtitleFactory,
			UINotification.Factory notificationFactory,
			UIChoice.Factory choiceFactory)
		{
			this.settings = settings;
			this.subCanvas = subCanvas;
			this.asyncManager = asyncManager;
			this.subtitleFactory = subtitleFactory;
			this.notificationFactory = notificationFactory;
			this.choiceFactory = choiceFactory;

			subCanvas.WindowsManager.Register(this);
		}

		private void Start()
		{
			Enable(false);
			SubtitlesContent.DestroyChildren();
			NotificationContent.DestroyChildren();
			ChoiceContent.DestroyChildren();

			DialogueTree.OnDialogueStarted += OnDialogueStarted;
			DialogueTree.OnDialoguePaused += OnDialoguePaused;
			DialogueTree.OnDialogueFinished += OnDialogueFinished;
			DialogueTree.OnSubtitlesRequest += OnSubtitlesRequest;
			DialogueTree.OnMultipleChoiceRequest += OnMultipleChoiceRequest;

			Button.onClick.AddListener(OnClick);
		}

		private void OnDestroy()
		{
			subCanvas.WindowsManager.UnRegister(this);

			DialogueTree.OnDialogueStarted -= OnDialogueStarted;
			DialogueTree.OnDialoguePaused -= OnDialoguePaused;
			DialogueTree.OnDialogueFinished -= OnDialogueFinished;
			DialogueTree.OnSubtitlesRequest -= OnSubtitlesRequest;
			DialogueTree.OnMultipleChoiceRequest -= OnMultipleChoiceRequest;

			Button?.onClick.RemoveListener(OnClick);
		}

		private void OnDialogueStarted(DialogueTree tree)
		{
			Enable(true);
		}
		private void OnDialoguePaused(DialogueTree tree) { }
		private void OnDialogueFinished(DialogueTree tree)
		{
			Enable(false);
			DispawnSubtitles();
			DispawnNotifications();
			Spacer.preferredHeight = 0;
		}

		private IEnumerator CalculateHeight()
		{
			//Canvas.ForceUpdateCanvases();

			yield return null;

			#region Resize Spacer
			Assert.AreNotEqual(SubtitlesContent.childCount, 0, "dialogue.SubtitlesContent.childCount == 0");

			int countSpaces = 1;

			var currentSubtitle = SubtitlesContent.GetChild(SubtitlesContent.childCount - 1) as RectTransform;
			float currentSubtitleHeight = LayoutUtility.GetPreferredHeight(currentSubtitle);

			float choiceHeight = LayoutUtility.GetPreferredHeight(ChoiceContent as RectTransform);
			ChoiceContent.gameObject.SetActive(choiceHeight != 0);
			if (ChoiceContent.gameObject.activeSelf)
			{
				countSpaces++;
			}

			float notificationsHeight = LayoutUtility.GetPreferredHeight(NotificationContent as RectTransform);
			NotificationContent.gameObject.SetActive(notificationsHeight != 0);
			if (NotificationContent.gameObject.activeSelf)
			{
				countSpaces++;
			}

			float contentSpaces = ContentLayoutGroup.spacing * countSpaces;
			float subtitlesSpacing = SubtitlesLayoutGroup.spacing;

			float height = (RectTransform.sizeDelta.y) - (currentSubtitleHeight + choiceHeight + notificationsHeight + (contentSpaces + subtitlesSpacing));//maxHeight - currentHeight
			Spacer.preferredHeight = height <= 0 ? 0 : height;
			#endregion

			yield return null;

			#region Scroll To Element Top Position
			var topPosition = currentSubtitle.position;
			topPosition.y += (currentSubtitle.sizeDelta.y / 2) + subtitlesSpacing;

			var destination = (Vector2)Content.transform.InverseTransformPoint(Content.position) - (Vector2)Content.transform.InverseTransformPoint(topPosition);
			destination.x = 0;

			DOTween.To(() => Content.anchoredPosition, x => Content.anchoredPosition = x, destination, 0.25f);
			#endregion
		}

		#region Notifications
		private void SpawnNotifications(ChoiceWrapper choiceWrapper)
		{
			choiceWrapper.notifications.ForEach((x) =>
			{
				var notification = notificationFactory.Create();

				notification.Text.text = x.text;
				notification.Text.color = x.basetColor;

				notification.transform.SetParent(NotificationContent);

				notifications.Add(notification);
			});

			if (!NotificationContent.gameObject.activeSelf && notifications.Count > 0)
			{
				NotificationContent.gameObject.SetActive(true);
			}
		}
		private void DispawnNotifications()
		{
			for (int i = notifications.Count - 1; i >= 0; i--)
			{
				notifications[i].DespawnIt();
				notifications.Remove(notifications[i]);
			}
		}
		#endregion

		#region Subtitles
		private IEnumerator WaitSubtitlesInput(SubtitlesRequestInfo info)
		{
			isWaitInput = info.waitForInput;
			if (info.waitForInput)
			{
				WaitIndicator.SetActive(true);
				while (isWaitInput)
				{
					if (Input.GetKeyDown(KeyCode.Space))
					{
						isWaitInput = false;
					}

					yield return null;
				}
				WaitIndicator.SetActive(false);
			}

			info.Continue();
		}
		private void SpawnSubtitles(ISheet sheet, string text)
		{
			ActorPortrait.gameObject.SetActive(sheet.Information.IsHasPortrait);
			ActorPortrait.sprite = sheet.Information.portrait;

			var subtitle = subtitleFactory.Create();

			subtitle.Text.text = $"{sheet.Information.Name} - {text}";

			subtitle.transform.SetParent(SubtitlesContent);

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
		private void OnSubtitlesRequest(SubtitlesRequestInfo info)
		{
			var actor = info.actor as IActor;

			Assert.IsNotNull(actor, "IActor == null");

			SpawnSubtitles(actor.GetSheet(), info.statement.Text);

			asyncManager.StartCoroutine(WaitSubtitlesInput(info));
		}
		#endregion

		#region Choice
		private IEnumerator WaitChoiceInput()
		{
			isWaitChoiceInput = true;

			while (isWaitChoiceInput)
			{
				for (int i = 1; i < 10; i++)//Alpha1-9
				{
					if (Input.GetKeyDown((KeyCode)(48 + i)))
					{
						if (i <= choices.Count)
						{
							OnChoiced(choices[i - 1].ui);
						}
					}
				}

				yield return null;
			}
		}
		private void FillChoice(int index, ChoiceWrapper wrapper)
		{
			switch (wrapper.choice.choiceConditionState)
			{
				case ChoiceConditionState.Normal:
				{
					wrapper.ui.Text.color = wrapper.choice.isSelected ? Color.gray : Color.white;
					wrapper.ui.Text.text = $"{index}. {GetConsequence()}{GetRequirements()}{wrapper.choice.statement.GetCurrent().Text}";//1. (Aligment or Lie-True) [Requirements or Action] Actor - Text.
					break;
				}
				case ChoiceConditionState.Inactive:
				{
					wrapper.ui.Text.color = new Color(0.35f, 0.35f, 0.35f);//dark gray
					wrapper.ui.Text.text = $"{index}. {GetConsequence()}[Conditions Not Met] {wrapper.choice.statement.GetCurrent().Text}";
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
						if (settings.showAlignmentRequirementsInDialogues)
						{
							requires += alignmentRequirement.alignmentRequired;
						}
						errors += "Specific alignment required";
					}
				});

				return string.IsNullOrEmpty(requires) ? (includeLabel ? errors : "") : $"[Requires {requires}] " + (includeLabel ? errors : "");
			}
			string GetConsequence()
			{
				string consequence = "";

				wrapper.choice.consequence.ForEach((x) =>
				{
					if (x is CommandSetAlignment commandAlignment)
					{
						if (settings.showAlignmentShiftsInDialogues)
						{
							consequence += commandAlignment.Target;
						}
						if (settings.showAlignmentShiftsNotificationsInDialogues)
						{
							wrapper.notifications.Add(new Notification() { text = $"You've performed a <color={Alignment.GetAlignmentColor(commandAlignment.Target).ToHEX()}>{commandAlignment.Target}</color> action" });

							if (commandAlignment.Current != commandAlignment.Forecast)
							{
								wrapper.notifications.Add(new Notification() { text = $"You new alignment is <color={Alignment.GetAlignmentColor(commandAlignment.Forecast).ToHEX()}>{commandAlignment.Forecast}</color>" });
							}
						}
					}
					else if (x is CommandAddExperience commandExperience)
					{
						if (settings.showExperiaencePointsGainedInDialogues)
						{
							wrapper.notifications.Add(new Notification() { text = $"Gained {commandExperience.exp} Experience", basetColor = new Color(0, 0.7f, 0) });//light green

							if (commandExperience.IsLevelChanged())
							{
								wrapper.notifications.Add(new Notification() { text = $"Level Up!", basetColor = new Color(0, 0.7f, 0) });//light green
							}
						}
					}
					else if (x is CommandAddItems commandItems)
					{
						string items = "";

						for (int i = 0; i < commandItems.Items.Count; i++)
						{
							items += commandItems.Items[i];

							if (i != commandItems.Items.Count - 1)
							{
								items += ", ";
							}
						}

						wrapper.notifications.Add(new Notification() { text = $"<color={new Color(0, 0.7f, 0).ToHEX()}>Items received:</color> {items}" });//light green
					}
				});
				consequence = !string.IsNullOrEmpty(consequence) ? $"({consequence}) " : "";//spaces

				return consequence;
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
		private void OnMultipleChoiceRequest(MultipleChoiceRequestInfo info)
		{
			cachedChoiceInfo = info;

			DespawnChoices();

			//fill
			for (int i = 0; i < info.choices.Count; i++)
			{
				Choice choice = info.choices[i] as Choice;

				UIChoice choiceUI = choiceFactory.Create();

				ChoiceWrapper wrapper = new ChoiceWrapper()
				{
					choice = choice,
					ui = choiceUI
				};

				FillChoice(i + 1, wrapper);

				choices.Add(wrapper);
				choiceUI.onButtonClick += OnChoiced;
				choiceUI.transform.SetParent(ChoiceContent);
			}

			if (!ChoiceContent.gameObject.activeSelf && choices.Count > 0)
			{
				ChoiceContent.gameObject.SetActive(true);
			}

			asyncManager.StartCoroutine(CalculateHeight());
			if (settings.isCanUseKeyAlpha)
			{
				asyncManager.StartCoroutine(WaitChoiceInput());
			}
		}
		private void OnChoiced(UIChoice choice)
		{
			isWaitChoiceInput = false;

			var wrapper = choices.Find((x) => x.ui == choice);

			if (wrapper.choice.choiceConditionState == ChoiceConditionState.Normal)
			{
				if (settings.isSayChoice)
				{
					var actor = cachedChoiceInfo.actor as IActor;
					Assert.IsNotNull(actor, "MultipleChoiceRequestInfo IActor == null");
					//SpawnSubtitles(actor, wrapper.option.Statement.Text);

					SpawnNotifications(wrapper);
				}

				int index = choices.IndexOf(wrapper);
				DespawnChoices();
				cachedChoiceInfo.SelectOption(index);
			}
		}
		#endregion

		private void OnClick()
		{
			isWaitInput = false;
		}


		[System.Serializable]
		public class Settings
		{
			public bool isCanUseKeyAlpha = true;
			[Space]
			public bool isSayChoice = true;
			[Space]
			public bool showAlignmentRequirementsInDialogues = true;
			public bool showAlignmentShiftsNotificationsInDialogues = true;
			public bool showAlignmentShiftsInDialogues = true;
			[Space]
			public bool showSkillCheckInDialogues = true;
			public bool showSkillChecksResultInDialogues = true;
			[Space]
			public bool showExperiaencePointsGainedInDialogues = true;

			//[System.Serializable]
			//public class SubtitleDelays
			//{
			//	public float characterDelay = 0.05f;
			//	public float sentenceDelay = 0.5f;
			//	public float commaDelay = 0.1f;
			//	public float finalDelay = 1.2f;
			//}
		}
	}

	public class ChoiceWrapper
	{
		public Choice choice;
		public UIChoice ui;

		public List<Notification> notifications = new List<Notification>();
	}

	public class Notification
	{
		public string text;
		public Color basetColor = new Color(0.7f, 0.7f, 0.7f);//light grey
	}
}