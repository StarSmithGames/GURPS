using Zenject;

public abstract class WindowAnimatedPoolable : WindowBasePoolable
{
	public TransformPopup Popup => popup;

	private TransformPopup popup;

	[Inject]
	private void Construct()
	{
		popup = new TransformPopup(transform, 0.25f, 0.25f);
		popup.SetOut();
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