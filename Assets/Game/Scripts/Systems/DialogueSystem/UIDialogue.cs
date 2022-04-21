using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Systems.DialogueSystem
{
	public class UIDialogue : MonoBehaviour
	{
		public UnityAction onContinueClick;

		[field: SerializeField] public TMPro.TextMeshProUGUI ActorSpeech { get; private set; }
		[field: SerializeField] public Image ActorPortrait { get; private set; }
		[field: Space]
		[field: SerializeField] public Button ContinueButton { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI ContinueText { get; private set; }
		[field: Space]
		[field: SerializeField] public Transform ChoiceContent { get; private set; }
		[field: Space]
		[field: SerializeField] public GameObject WaitIndicator { get; private set; }


		public void Start()
		{
			ContinueButton.onClick.AddListener(OnContinueClicked);
		}

		private void OnDestroy()
		{
			if(ContinueButton != null)
			{
				ContinueButton.onClick.RemoveListener(OnContinueClicked);
			}
		}

		public void Enable(bool trigger)
		{
			gameObject.SetActive(trigger);
		}

		public void EnableContinueButton(bool trigger)
		{
			ContinueButton.gameObject.SetActive(trigger);
		}


		private void OnContinueClicked()
		{
			onContinueClick?.Invoke();
		}
	}
}