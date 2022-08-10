using UnityEngine;
using UnityEngine.Events;

using Zenject;

public abstract class WindowBase : MonoBehaviour, IWindow
{
	private bool isShowing = false;
	public bool IsShowing => isShowing;

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
}

public abstract class WindowBasePoolable : PoolableObject, IWindow
{
	protected bool isShowing = false;
	public bool IsShowing => isShowing;

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
}