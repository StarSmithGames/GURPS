using Game.Entities;
using Game.Managers.CharacterManager;
using Game.Managers.PartyManager;
using Game.Systems.CameraSystem;
using Game.UI;
using Game.UI.Windows;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

namespace Game.Managers.PartyManager
{
	public class WindowParty : WindowBase
	{
		private List<UIAvatar> avatars = new List<UIAvatar>();

		private SignalBus signalBus;
		private UIAvatar.Factory avatarFactory;
		private PartyManager partyManager;
		private UISubCanvas subCanvas;
		private CameraController cameraController;

		[Inject]
		private void Construct(SignalBus signalBus,
			UIAvatar.Factory avatarFactory,
			PartyManager partyManager,
			UISubCanvas subCanvas,
			CameraController cameraController)
		{
			this.signalBus = signalBus;
			this.avatarFactory = avatarFactory;
			this.partyManager = partyManager;
			this.subCanvas = subCanvas;
			this.cameraController = cameraController;
		}

		private void Start()
		{
			transform.DestroyChildren();

			subCanvas.WindowsManager.Register(this);

			UpdateAvatars();

			UpdateLeader();

			signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
			signalBus?.Subscribe<SignalPartyChanged>(OnPartyChanged);
		}

		private void OnDestroy()
		{
			subCanvas.WindowsManager.UnRegister(this);

			signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);

			avatars.ForEach((x) =>
			{
				x.onClicked -= OnAvatarClicked;
				x.onDoubleClicked -= OnAvatarDoubleClicked;
			});
		}

		private void UpdateAvatars()
		{
			var characters = partyManager.PlayerParty.Characters;
			int diff = characters.Count - avatars.Count;

			if (diff != 0)
			{
				if(diff > 0)//add
				{
					for (int i = 0; i < diff; i++)
					{
						CreateAvatar();
					}
				}
				else//rm
				{
					for (int i = 0; i < -diff; i++)
					{
						RemoveLastAvatar();
					}
				}

				void CreateAvatar()
				{
					var avatar = avatarFactory.Create();

					avatar.onClicked += OnAvatarClicked;
					avatar.onDoubleClicked += OnAvatarDoubleClicked;
					avatar.transform.SetParent(transform);
					avatar.transform.localScale = Vector3.one;
					avatars.Add(avatar);
				}

				void RemoveLastAvatar()
				{
					var avatar = avatars.Last();

					avatar.onClicked -= OnAvatarClicked;
					avatar.onDoubleClicked -= OnAvatarDoubleClicked;
					avatars.Remove(avatar);
					avatar.DespawnIt();
				}
			}

			for (int i = 0; i < avatars.Count; i++)
			{
				avatars[i].SetCharacter(characters[i]);
			}
		}

		private void UpdateLeader()
		{
			int index = partyManager.PlayerParty.LeaderPartyIndex;

			avatars.ForEach((x) => x.SetFrame(false));
			avatars[index].SetFrame(true);
		}

		private void OnAvatarClicked(UIAvatar avatar)
		{
			int index = avatars.IndexOf(avatar);
			var character = partyManager.PlayerParty.Characters[index];

			//if (!character.InBattle)
			{
				partyManager.PlayerParty.SetLeader(index);
			}
		}

		private void OnAvatarDoubleClicked(UIAvatar avatar)
		{
			OnAvatarClicked(avatar);
			cameraController.CameraToHome();
		}

		private void OnPartyChanged(SignalPartyChanged signal)
		{
			UpdateAvatars();
		}

		private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
		{
			UpdateLeader();
		}
	}
}