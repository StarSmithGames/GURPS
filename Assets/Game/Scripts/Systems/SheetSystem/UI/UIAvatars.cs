using Game.Managers.CharacterManager;
using Game.Managers.PartyManager;
using Game.Systems.CameraSystem;

using System.Collections.Generic;

using UnityEngine;

using Zenject;

public class UIAvatars : MonoBehaviour
{
	private List<UIAvatar> avatars = new List<UIAvatar>();

	private SignalBus signalBus;
	private CharacterManager characterManager;
	private CameraController cameraController;
	private UIAvatar.Factory avatarFactory;

	[Inject]
	private void Construct(SignalBus signalBus, CharacterManager characterManager, CameraController cameraController, UIAvatar.Factory avatarFactory)
	{
		this.signalBus = signalBus;
		this.characterManager = characterManager;
		this.cameraController = cameraController;
		this.avatarFactory = avatarFactory;
	}

	private void OnDestroy()
	{
		signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);

		avatars.ForEach((x) =>
		{
			x.onClicked -= OnAvatarClicked;
			x.onDoubleClicked -= OnAvatarDoubleClicked;
		});
	}

	private void Start()
	{
		transform.DestroyChildren();

		UpdateAvatars();

		UpdateLeader();

		signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
	}

	private void UpdateAvatars()
	{
		//characterManager.CurrentParty.Characters.ForEach((x) =>
		//{
		//	CreateAvatar(x);
		//});

		//void CreateAvatar(CharacterModel character)
		//{
		//	var avatar = avatarFactory.Create();

		//	avatar.onClicked += OnAvatarClicked;
		//	avatar.onDoubleClicked += OnAvatarDoubleClicked;

		//	avatar.SetCharacter(character);

		//	avatar.transform.parent = transform;
		//	avatar.transform.localScale = Vector3.one;

		//	avatars.Add(avatar);
		//}
	}

	private void UpdateLeader()
	{
		//int index = characterManager.CurrentParty.LeaderPartyIndex;

		//avatars.ForEach((x) => x.SetFrame(false));
		//avatars[index].SetFrame(true);
	}


	private void OnAvatarClicked(UIAvatar avatar)
	{
		//int index = avatars.IndexOf(avatar);
		//var character = characterManager.CurrentParty.Characters[index];

		//if (!character.InBattle)
		//{
		//	characterManager.CurrentParty.SetLeader(index);
		//}
	}

	private void OnAvatarDoubleClicked(UIAvatar avatar)
	{
		OnAvatarClicked(avatar);
		cameraController.CameraToHome();
	}

	private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
	{
		UpdateLeader();
	}
}