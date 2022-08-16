using Cinemachine;

using Game.Entities;
using Game.Managers.CharacterManager;
using Game.Managers.InputManager;
using Game.Systems.InteractionSystem;

using UnityEngine;

using Zenject;

namespace Game.Systems.CameraSystem
{
	public class CameraVisionMap : CameraVision
	{
		private PlayerRTS Player => characterManager.PlayerRTS;

		private CharacterManager characterManager;

		public CameraVisionMap(SignalBus signalBus,
			GlobalSettings settings,
			CinemachineBrain brain,
			InputManager inputManager,
			CharacterManager characterManager) : base(signalBus, brain, inputManager)
		{
			this.settings = settings.cameraVisionMap;
			this.characterManager = characterManager;
		}

		protected override void HandleMouseClick(Vector3 point)
		{
			if (inputManager.IsLeftMouseButtonPressed())
			{
				if (CurrentObserve != null)//Interaction
				{
					if (inputManager.IsLeftMouseButtonDown())
					{
						if (CurrentObserve is IInteractable interactable)
						{
							Interactor.ABInteraction(Player, interactable);
						}
					}
				}
				else//Targeting
				{
					if (IsCanHoldMouse || inputManager.IsLeftMouseButtonDown())
					{
						if (IsMouseHit && !IsUI)
						{
							Player.SetDestination(point);
						}
					}
				}
			}
			else if(inputManager.IsRightMouseButtonPressed())//ContextMenu
			{
				if (inputManager.IsRightMouseButtonDown())
				{

				}
			}
		}
	}
}