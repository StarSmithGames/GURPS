using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Zenject;
using Sirenix.OdinInspector;

namespace Game.UI.Windows
{
	public interface IWindow : IUI
	{
		void Show(UnityAction callback = null);
		void Hide(UnityAction callback = null);
	}

	public abstract class WindowBase : MonoBehaviour, IWindow
	{
		public virtual bool IsShowing { get; protected set; }

		[field: SerializeField] public CanvasGroup CanvasGroup { get; protected set; }

		public virtual void Show(UnityAction callback = null)
		{
			CanvasGroup.alpha = 0f;
			CanvasGroup.Enable(true, false);
			IsShowing = true;

			Sequence sequence = DOTween.Sequence();

			sequence
				.Append(CanvasGroup.DOFade(1f, 0.2f))
				.AppendCallback(() => callback?.Invoke());
		}

		public virtual void Hide(UnityAction callback = null)
		{
			Sequence sequence = DOTween.Sequence();

			sequence
				.Append(CanvasGroup.DOFade(0f, 0.15f))
				.AppendCallback(() =>
				{
					CanvasGroup.Enable(false);
					IsShowing = false;
					callback?.Invoke();
				});
		}

		public virtual void Enable(bool trigger)
		{
			CanvasGroup.Enable(trigger);
			IsShowing = trigger;
		}

		[Button]
		private void OpenClose()
		{
			Enable(CanvasGroup.alpha == 0f ? true : false);
		}
	}

	public abstract class WindowBasePoolable : PoolableObject, IWindow
	{
		public virtual bool IsShowing { get; protected set; }

		public virtual void Show(UnityAction callback = null)
		{
			IsShowing = true;
			gameObject.SetActive(true);
			callback?.Invoke();
		}

		public virtual void Hide(UnityAction callback = null)
		{
			DespawnIt();
			IsShowing = false;
			callback?.Invoke();
		}

		public virtual void Enable(bool trigger)
		{
			gameObject.SetActive(trigger);
			IsShowing = trigger;
		}
	}

	public abstract class WindowAnimatedPoolable : WindowBasePoolable
	{
		public bool IsInProcess { get; private set; }
		public TransformPopup Popup { get; private set; }


		[Inject]
		private void Construct()
		{
			Popup = new TransformPopup(transform, 0.25f, 0.25f);
			Popup.SetOut();
		}

		public void ShowPopup(UnityAction callback = null)
		{
			IsInProcess = true;
			Enable(true);
			Popup.PopIn(callback, () => IsInProcess = false);
		}

		public void HidePopup(UnityAction callback = null)
		{
			IsInProcess = true;

			Popup.PopOut(onComplete: () =>
			{
				Enable(false);
				IsInProcess = false;
				callback?.Invoke();
			});
		}
	}
}