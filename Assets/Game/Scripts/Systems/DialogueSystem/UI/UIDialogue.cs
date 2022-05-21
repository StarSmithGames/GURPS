using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Systems.DialogueSystem
{
	public class UIDialogue : MonoBehaviour
	{
		public RectTransform RectTransform => transform as RectTransform;

		[field: SerializeField] public LayoutElement Spacer { get; private set; }
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


		public void Enable(bool trigger)
		{
			gameObject.SetActive(trigger);
		}
	}
}