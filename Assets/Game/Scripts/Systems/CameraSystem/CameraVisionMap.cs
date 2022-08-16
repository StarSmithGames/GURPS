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
		private IPlayer player;

		public CameraVisionMap(SignalBus signalBus,
			GlobalSettings settings,
			CinemachineBrain brain,
			InputManager inputManager,
			IPlayer player) : base(signalBus, brain, inputManager)
		{
			this.settings = settings.cameraVisionMap;
			this.player = player;
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
							Interactor.ABInteraction(player.RTSModel, interactable);
						}
					}
				}
				else//Targeting
				{
					if (IsCanHoldMouse || inputManager.IsLeftMouseButtonDown())
					{
						if (IsMouseHit && !IsUI)
						{
							player.RTSModel.SetDestination(point);
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