using Cinemachine;

using Game.Entities;
using Game.Managers.CharacterManager;
using Game.Managers.InputManager;
using Game.Systems.InteractionSystem;

using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

namespace Game.Systems.CameraSystem
{
	public class CameraVisionMap : IInitializable, IDisposable, ITickable, ICameraVision
	{
		public bool IsCanHoldMouse { get; private set; }
		public bool IsMouseHit { get; private set; }
		public bool IsUI { get; private set; }

		private IObservable CurrentObserve
		{
			get => currentEntity;
			set
			{
				if (currentEntity != value)
				{
					currentEntity?.EndObserve();
					currentEntity = value;
					currentEntity?.StartObserve();

					OnHoverObserveChanged();
				}
				else
				{
					currentEntity?.Observe();
				}
			}
		}
		private IObservable currentEntity = null;

		private PlayerRTS Player => characterManager.PlayerRTS;

		private SignalBus signalBus;
		private CinemachineBrain brain;
		private InputManager inputManager;
		private Settings settings;
		private CharacterManager characterManager;

		public CameraVisionMap(SignalBus signalBus, CinemachineBrain brain, InputManager inputManager, GlobalSettings settings, CharacterManager characterManager)
		{
			this.signalBus = signalBus;
			this.brain = brain;
			this.inputManager = inputManager;
			this.settings = settings.cameraVisionMap;
			this.characterManager = characterManager;
		}

		public void Initialize()
		{
			IsCanHoldMouse = settings.isCanHoldMouse;
		}

		public void Dispose()
		{
		
		}

		public void Tick()
		{
			IsUI = EventSystem.current.IsPointerOverGameObject();

			RaycastHit hit;
			Ray mouseRay = brain.OutputCamera.ScreenPointToRay(inputManager.GetMousePosition());
			IsMouseHit = Physics.Raycast(mouseRay, out hit, settings.raycastLength, settings.raycastLayerMask);
			IsUI = EventSystem.current.IsPointerOverGameObject();
			Vector3 point = hit.point;

			//Looking
			CurrentObserve = IsMouseHit && !IsUI ? hit.transform.root.GetComponent<IObservable>() : null;

			HandleHover(point);
			HandleMouseClick(point);
		}


		private void HandleMouseClick(Vector3 point)
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

		private void HandleHover(Vector3 point)
		{

		}

		private void OnHoverObserveChanged()
		{

		}


		[System.Serializable]
		public class Settings
		{
			public bool isCanHoldMouse = true;

			public LayerMask raycastLayerMask = ~0;
			public float raycastLength = 100f;
		}
	}
}