using Cinemachine;

using Game.Managers.CharacterManager;
using Game.Managers.GameManager;

using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

public class CameraVision : IInitializable, IDisposable, ITickable
{
	public bool IsCanHoldMouse { get; private set; }
	public bool IsCamDrawPath { get; private set; }

	private IObservable CurrentEntity
	{
		get => currentEntity;
		set
		{
			if (currentEntity != value)
			{
				currentEntity?.EndObserve();
				currentEntity = value;
				currentEntity?.StartObserve();
			}
			else
			{
				currentEntity?.Observe();
			}
		}
	}
	private IObservable currentEntity = null;

	private CharacterParty party;

	private SignalBus signalBus;
	private CinemachineBrain brain;
	private InputManager inputManager;
	private CharacterManager characterManager;
	private Settings settings;

	public CameraVision(SignalBus signalBus, CinemachineBrain brain, InputManager inputManager, CharacterManager characterManager, GlobalSettings settings)
	{
		this.signalBus = signalBus;
		this.brain = brain;
		this.inputManager = inputManager;
		this.characterManager = characterManager;
		this.settings = settings.cameraVision;
	}

	public void Initialize()
	{
		IsCanHoldMouse = settings.isCanHoldMouse;

		signalBus?.Subscribe<SignalGameStateChanged>(OnGameStateChanged);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalGameStateChanged>(OnGameStateChanged);
	}

	public void Tick()
	{
		RaycastHit hit;
		Ray mouseRay = brain.OutputCamera.ScreenPointToRay(inputManager.GetMousePosition());

		//Looking
		if (Physics.Raycast(mouseRay, out hit, settings.raycastLength, settings.raycastLayerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject())
		{
			CurrentEntity = hit.transform.root.GetComponent<IObservable>();
		}
		else
		{
			CurrentEntity = null;
		}

		//MouseHolding
		if (inputManager.IsLeftMouseButtonPressed())
		{
			if(CurrentEntity != null)
			{
				if (inputManager.IsLeftMouseButtonDown())
				{
					characterManager.Party.CurrentCharacter.InteractWith(CurrentEntity);
				}
			}
			else
			{
				//Targeting
				if (IsCanHoldMouse || inputManager.IsLeftMouseButtonDown())
				{
					if (Physics.Raycast(mouseRay, out hit, settings.raycastLength, settings.raycastLayerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject())
					{
						characterManager.Party.CurrentCharacter.Controller.SetDestination(hit.point);
					}
				}
			}
		}
	}

	private void OnGameStateChanged(SignalGameStateChanged signal)
	{
		if(signal.newGameState == GameState.Battle)
		{
			IsCanHoldMouse = false;
			IsCamDrawPath = true;
		}
	}
	

	[System.Serializable]
	public class Settings
	{
		public bool isCanHoldMouse = true;

		public LayerMask raycastLayerMask = ~0;
		public float raycastLength = 100f;
	}
}