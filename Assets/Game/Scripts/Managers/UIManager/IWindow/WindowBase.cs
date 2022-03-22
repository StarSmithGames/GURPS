using UnityEngine;

using Zenject;

public abstract class WindowBase : MonoBehaviour, IWindow
{
	private bool isShowing = false;
	public bool IsShowing => isShowing;

	public virtual void Show()
	{
		gameObject.SetActive(true);

		isShowing = true;
	}

	public virtual void Hide()
	{
		gameObject.SetActive(false);

		isShowing = false;
	}
}

public abstract class WindowBasePoolable : PoolableObject, IWindow
{
	protected bool isShowing = false;
	public bool IsShowing => isShowing;

	public virtual void Show()
	{
		gameObject.SetActive(true);

		isShowing = true;
	}

	public virtual void Hide()
	{
		gameObject.SetActive(false);

		isShowing = false;
	}
}