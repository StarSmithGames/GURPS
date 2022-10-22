using Game.Managers.GameManager;
using Game.Systems.CameraSystem;
using Game.UI;
using Game.UI.CanvasSystem;
using Game.UI.Windows;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

namespace Game.Managers.PartyManager
{
	public class WindowParty : WindowBase
	{
		[field: SerializeField] public Transform Content { get; private set; }

		private List<UIAvatar> avatars = new List<UIAvatar>();

		private SignalBus signalBus;
		private UIAvatar.Factory avatarFactory;
		private GameManager.GameManager gameManager;
		private PartyManager partyManager;
		private UISubCanvas subCanvas;
		private CameraController cameraController;

		[Inject]
		private void Construct(SignalBus signalBus,
			UIAvatar.Factory avatarFactory,
			GameManager.GameManager gameManager,
			PartyManager partyManager,
			UISubCanvas subCanvas,
			CameraController cameraController)
		{
			this.signalBus = signalBus;
			this.avatarFactory = avatarFactory;
			this.gameManager = gameManager;
			this.partyManager = partyManager;
			this.subCanvas = subCanvas;
			this.cameraController = cameraController;
		}

		private void Start()
		{
			Content.DestroyChildren();

			subCanvas.WindowsRegistrator.Registrate(this);

			signalBus?.Subscribe<SignalGameStateChanged>(OnGameStateChanged);
			signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
			signalBus?.Subscribe<SignalPartyChanged>(OnPartyChanged);
		}

		private void OnDestroy()
		{
			subCanvas.WindowsRegistrator.UnRegistrate(this);

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

			CollectionExtensions.Resize(characters, avatars,
			() =>
			{
				var avatar = avatarFactory.Create();

				avatar.onClicked += OnAvatarClicked;
				avatar.onDoubleClicked += OnAvatarDoubleClicked;
				avatar.transform.SetParent(Content);
				avatar.transform.localScale = Vector3.one;
				return avatar;
			},
			() =>
			{
				var avatar = avatars.Last();

				avatar.onClicked -= OnAvatarClicked;
				avatar.onDoubleClicked -= OnAvatarDoubleClicked;
				avatar.DespawnIt();
				return avatar;
			});

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

		private void OnGameStateChanged(SignalGameStateChanged signal)
		{
			if(signal.newGameState == GameState.Gameplay)
			{
				UpdateAvatars();
				UpdateLeader();
			}
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