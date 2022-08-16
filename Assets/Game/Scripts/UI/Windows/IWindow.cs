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

	public abstract class WindowAnimatedPoolable : WindowBasePoolable
	{
		public TransformPopup Popup { get; private set; }


		[Inject]
		private void Construct()
		{
			Popup = new TransformPopup(transform, 0.25f, 0.25f);
			Popup.SetOut();
		}

		public void ShowPopup()
		{
			isShowing = true;

			Popup.PopIn(onStart: () => base.Show());
		}

		public void HidePopup()
		{
			Popup.PopOut(onComplete: () => base.Hide());
		}
	}
}