using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using Zenject;

namespace Game.Systems.DialogueSystem
{
	public class Barker : MonoBehaviour
	{
		public bool IsShowing { get; private set; }

		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }

		private Settings settings;
		private Cinemachine.CinemachineBrain brain;

		[Inject]
		private void Construct(Settings settings, Cinemachine.CinemachineBrain brain)
		{
			this.settings = settings;
			this.brain = brain;
		}

		private void Start()
		{
			Enable(false);
		}

		private void Update()
		{
			if (IsShowing)
			{
				transform.rotation = brain.OutputCamera.transform.rotation;
			}
		}

		public void Enable(bool trigger)
		{
			IsShowing = trigger;
			gameObject.SetActive(trigger);
		}

		public void Show()
		{
			Sequence sequence = DOTween.Sequence();

			CanvasGroup.alpha = 0f;
			Enable(true);

			sequence
				.Append(CanvasGroup.DOFade(1, 0.3f))
				.AppendInterval(settings.waitTime)
				.AppendCallback(Hide);
		}

		public void Hide()
		{
			Sequence sequence = DOTween.Sequence();

			CanvasGroup.alpha = 1f;

			sequence
				.Append(CanvasGroup.DOFade(0, 0.2f))
				.AppendCallback(() =>
				{
					Enable(false);
				});
		}

		[System.Serializable]
		public class Settings
		{
			public float waitTime = 2f;
		}
	}
}