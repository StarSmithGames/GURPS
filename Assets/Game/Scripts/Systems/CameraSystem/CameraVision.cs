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
	private GameManager gameManager;
	private CharacterManager characterManager;
	private Settings settings;

	public CameraVision(SignalBus signalBus, CinemachineBrain brain, InputManager inputManager, GameManager gameManager, CharacterManager characterManager, GlobalSettings settings)
	{
		this.signalBus = signalBus;
		this.brain = brain;
		this.inputManager = inputManager;
		this.gameManager = gameManager;
		this.characterManager = characterManager;
		this.settings = settings.cameraVision;
	}

	public void Initialize()
	{
		IsCanHoldMouse = settings.isCanHoldMouse;

		signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
	}

	public void Tick()
	{
		RaycastHit hit;
		Ray mouseRay = brain.OutputCamera.ScreenPointToRay(inputManager.GetMousePosition());
		bool isHit = Physics.Raycast(mouseRay, out hit, settings.raycastLength, settings.raycastLayerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject();

		var leaderCharacter = characterManager.CurrentParty.LeaderParty;

		//Looking
		CurrentEntity = isHit ? hit.transform.root.GetComponent<IObservable>() : null;


		//MouseHolding
		if (inputManager.IsLeftMouseButtonPressed())
		{
			if(CurrentEntity != null)
			{
				if (inputManager.IsLeftMouseButtonDown())
				{
					leaderCharacter.InteractWith(CurrentEntity);
				}
			}
			else
			{
				//Targeting
				if (IsCanHoldMouse || inputManager.IsLeftMouseButtonDown())
				{
					if (isHit)
					{
						if (!leaderCharacter.InBattle || leaderCharacter.InBattle && !leaderCharacter.Controller.IsHasTarget)
						{
							leaderCharacter.Navigation.SetTarget(hit.point);
							leaderCharacter.Controller.SetDestination(hit.point);
						}
					}
				}
			}
		}


		if (isHit)
		{
			if (leaderCharacter.InBattle)
			{
				if (!leaderCharacter.Controller.IsHasTarget)
				{
					leaderCharacter.Navigation.SetTarget(hit.point);
				}
			}
		}
	}

	private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
	{
		IsCanHoldMouse = signal.leader.InBattle ? false : settings.isCanHoldMouse;
	}
	

	[System.Serializable]
	public class Settings
	{
		public bool isCanHoldMouse = true;

		public LayerMask raycastLayerMask = ~0;
		public float raycastLength = 100f;
	}
}