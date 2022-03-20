using Game.Managers.CharacterManager;

using System;

using Zenject;

public class UIHandlerTEST : IInitializable, IDisposable
{
	private CharacterManager characterManager;

	private UIManager uiManager;

	public UIHandlerTEST(UIManager uiManager, CharacterManager characterManager)
	{
		this.characterManager = characterManager;
		this.uiManager = uiManager;
	}

	public void Initialize()
	{
		uiManager.avatars.ForEach((x) => x.onClicked += OnAvatarClicked);
	}

	public void Dispose()
	{
		uiManager.avatars.ForEach((x) => x.onClicked -= OnAvatarClicked);
	}


	private void OnAvatarClicked(UIAvatar avatar)
	{
		characterManager.SetCharacter(uiManager.avatars.IndexOf(avatar));

		uiManager.avatars.ForEach((x) => x.SetFrame(false));
		avatar.SetFrame(true);
	}
}