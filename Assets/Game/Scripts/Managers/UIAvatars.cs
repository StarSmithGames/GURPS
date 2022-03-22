using Game.Managers.CharacterManager;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class UIAvatars : MonoBehaviour
{
	[SerializeField] private List<UIAvatar> avatars = new List<UIAvatar>();

	private CharacterManager characterManager;

	[Inject]
	private void Construct(CharacterManager characterManager)
	{
		this.characterManager = characterManager;

		avatars.ForEach((x) => x.onClicked += OnAvatarClicked);
	}

	private void OnDestroy()
	{
		avatars.ForEach((x) => x.onClicked -= OnAvatarClicked);
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
}