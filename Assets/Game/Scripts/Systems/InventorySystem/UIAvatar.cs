using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

public class UIAvatar : MonoBehaviour
{
	public UnityAction<UIAvatar> onClicked;

	public Button background;
	[Space]
	public Image frameMain;
	public Image frameSpare;

	[Inject]
	private void Construct()
	{
		background.onClick.AddListener(OnClick);
	}

	private void OnDestroy()
	{
		background.onClick.RemoveListener(OnClick);
	}

	public void SetFrame(bool isLeader)
	{
		frameMain.enabled = isLeader;
		frameSpare.enabled = !isLeader;
	}

	private void OnClick()
	{
		onClicked?.Invoke(this);
	}
}