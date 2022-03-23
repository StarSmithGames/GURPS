using Game.Managers.CharacterManager;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class UIAvatars : MonoBehaviour
{
	[SerializeField] private List<UIAvatar> avatars = new List<UIAvatar>();

	private CharacterManager characterManager;
	private CameraController cameraController;

	[Inject]
	private void Construct(CharacterManager characterManager, CameraController cameraController)
	{
		this.characterManager = characterManager;
		this.cameraController = cameraController;

		avatars.ForEach((x) =>
		{
			x.onClicked += OnAvatarClicked;
			x.onDoubleClicked += OnAvatarDoubleClicked;
		});
	}

	private void OnDestroy()
	{
		avatars.ForEach((x) =>
		{
			x.onClicked -= OnAvatarClicked;
			x.onDoubleClicked -= OnAvatarDoubleClicked;
		});
	}


	public void SetAvatarFrame(int index)
	{
		avatars.ForEach((x) => x.SetFrame(false));
		avatars[index].SetFrame(true);
	}

	private void OnAvatarClicked(UIAvatar avatar)
	{
		int index = avatars.IndexOf(avatar);
		characterManager.Party.SetCharacter(index);
	}

	private void OnAvatarDoubleClicked(UIAvatar avatar)
	{
		cameraController.CameraHome();
	}
}