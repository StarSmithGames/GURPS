using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

public class UIAvatar : MonoBehaviour
{
	public UnityAction<UIAvatar> onClicked;
	public UnityAction<UIAvatar> onDoubleClicked;

	public UIButtonPointer background;
	[Space]
	public Image frameMain;
	public Image frameSpare;

	[Inject]
	private void Construct()
	{
		background.onClickChanged += OnClick;
	}

	private void OnDestroy()
	{
		if(background != null)
		{
			background.onClickChanged -= OnClick;
		}
	}

	public void SetFrame(bool isLeader)
	{
		frameMain.enabled = isLeader;
		frameSpare.enabled = !isLeader;
	}

	private void OnClick(int count)
	{
		if(count == 1)
		{
			onClicked?.Invoke(this);
		}
		else if(count > 1)
		{
			onDoubleClicked?.Invoke(this);
		}
	}
}