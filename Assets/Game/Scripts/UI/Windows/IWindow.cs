using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.UI.Windows
{
	public interface IWindow : IUI
	{
		void Show(UnityAction callback = null);
		void Hide(UnityAction callback = null);
	}

	public abstract class WindowBase : MonoBehaviour, IWindow
	{
		public virtual bool IsShowing
		{
			get => isShowing;
			protected set => isShowing = value;
		}
		protected bool isShowing = false;

		public virtual void Show(UnityAction callback = null)
		{
			gameObject.SetActive(true);
			isShowing = true;
		}

		public virtual void Hide(UnityAction callback = null)
		{
			gameObject.SetActive(false);
			isShowing = false;
		}

		public virtual void Enable(bool trigger)
		{
			gameObject.SetActive(trigger);
			IsShowing = trigger;
		}
	}

	public abstract class WindowBasePoolable : PoolableObject, IWindow
	{
		public virtual bool IsShowing
		{
			get => isShowing;
			protected set => isShowing = value;
		}
		protected bool isShowing = false;

		public virtual void Show(UnityAction callback = null)
		{
			isShowing = true;
			gameObject.SetActive(true);
			callback?.Invoke();
		}

		public virtual void Hide(UnityAction callback = null)
		{
			DespawnIt();
			isShowing = false;
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