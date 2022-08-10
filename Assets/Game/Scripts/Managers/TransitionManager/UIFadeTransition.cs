using UnityEngine;

using DG.Tweening;

using Zenject;
using UnityEngine.Events;

namespace Game.Managers.TransitionManager
{
	public class UIFadeTransition : PoolableObject, ITransition
	{
		public bool IsInProgress { get; private set; }

		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }

		private float inTime = 0.2f;
		private float outTime = 0.15f;

		public void In(UnityAction callback = null)
		{
			IsInProgress = true;

			CanvasGroup.alpha = 0f;

			CanvasGroup.interactable = true;
			CanvasGroup.blocksRaycasts = true;

			CanvasGroup.DOFade(1, inTime)
				.OnComplete(() => callback?.Invoke());
		}

		public void Out(UnityAction callback = null)
		{
			CanvasGroup.DOFade(0, outTime)
				.OnComplete(() =>
			{
				CanvasGroup.interactable = false;
				CanvasGroup.blocksRaycasts = false;

				IsInProgress = false;

				DespawnIt();

				callback?.Invoke();
			});
		}

		public void Terminate()
		{
			CanvasGroup.DOKill(true);

			CanvasGroup.interactable = false;
			CanvasGroup.blocksRaycasts = false;

			IsInProgress = false;

			DespawnIt();
		}

		public class Factory : PlaceholderFactory<UIFadeTransition> { }
	}
}