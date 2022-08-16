using UnityEngine;

namespace Game.UI
{
	public interface IUI
	{
		bool IsShowing { get; }
		void Enable(bool trigger);
	}

	public abstract class UIBase : MonoBehaviour, IUI
	{
		public virtual bool IsShowing
		{
			get => isShowing;
			protected set => isShowing = value;
		}
		protected bool isShowing = false;

		public virtual void Enable(bool trigger)
		{
			gameObject.SetActive(trigger);
			IsShowing = trigger;
		}
	}

	public abstract class UIPollable : PoolableObject, IUI
	{
		public virtual bool IsShowing
		{
			get => isShowing;
			protected set => isShowing = value;
		}
		protected bool isShowing = false;

		public virtual void Enable(bool trigger)
		{
			gameObject.SetActive(trigger);
			IsShowing = trigger;
		}
	}
}