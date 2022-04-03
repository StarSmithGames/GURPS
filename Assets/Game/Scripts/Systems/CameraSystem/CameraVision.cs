using Cinemachine;

using Game.Entities;
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
	private Character leader;
	private bool isMouseHit;

	private SignalBus signalBus;
	private CinemachineBrain brain;
	private InputManager inputManager;
	private GameManager gameManager;
	private CharacterManager characterManager;
	private Settings settings;
	private UIManager uiManager;

	public CameraVision(SignalBus signalBus,
		CinemachineBrain brain,
		InputManager inputManager,
		GameManager gameManager,
		CharacterManager characterManager,
		GlobalSettings settings,
		UIManager uiManager)
	{
		this.signalBus = signalBus;
		this.brain = brain;
		this.inputManager = inputManager;
		this.gameManager = gameManager;
		this.characterManager = characterManager;
		this.settings = settings.cameraVision;
		this.uiManager = uiManager;
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
		isMouseHit = Physics.Raycast(mouseRay, out hit, settings.raycastLength, settings.raycastLayerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject();
		Vector3 point = hit.point;
		leader = characterManager.CurrentParty.LeaderParty;

		//Looking
		CurrentEntity = isMouseHit ? hit.transform.root.GetComponent<IObservable>() : null;

		//MouseHolding
		if (inputManager.IsLeftMouseButtonPressed())
		{
			if(CurrentEntity != null)
			{
				if (inputManager.IsLeftMouseButtonDown())
				{
					leader.InteractWith(CurrentEntity);
				}
			}
			else
			{
				//Targeting
				if (IsCanHoldMouse || inputManager.IsLeftMouseButtonDown())
				{
					if (isMouseHit)
					{
						if (!leader.InBattle)
						{
							leader.Controller.SetDestination(point);
						}
						else if(leader.InBattle && !leader.Controller.IsHasTarget)
						{
							if(leader.Sheet.Stats.Move.CurrentValue >= 0.1f)
							{
								leader.Controller.SetDestination(point, maxPathDistance: leader.Sheet.Stats.Move.CurrentValue);
							}
						}
					}
				}
			}
		}

		if (isMouseHit)
		{
			if (leader.InBattle)
			{
				if (!leader.Controller.IsHasTarget)
				{
					leader.Navigation.SetTarget(point, maxPathDistance: leader.Sheet.Stats.Move.CurrentValue);
				}
			}

			TryShowTooltipRuler();
		}

		ValidatePath(point);
	}

	private void TryShowTooltipRuler()
	{
		if (leader.InBattle && !leader.Controller.IsHasTarget)
		{
			uiManager.Tooltip.SetRulerText(Math.Round(leader.Navigation.CurrentPath.Distance, 2) + SymbolCollector.METRE.ToString());
			uiManager.Tooltip.EnableRuler(true);
		}
		else
		{
			if (uiManager.Tooltip.IsRulerShowing)
			{
				uiManager.Tooltip.EnableRuler(false);
			}
		}
	}

	private void ValidatePath(Vector3 point)
	{
		float pathDistance = (float)Math.Round(leader.Navigation.CurrentPath.Distance, 2);

		bool isInvalidTarget = !isMouseHit || !leader.Navigation.NavMeshAgent.IsPathValid(point);
		bool isNotEnoughMovement = leader.InBattle && !leader.Controller.IsHasTarget && (leader.Sheet.Stats.Move.CurrentValue < pathDistance);

		if (isInvalidTarget || isNotEnoughMovement)
		{
			if (isInvalidTarget)
			{
				uiManager.Tooltip.SetMessage(TooltipSystem.MessageType.InvalidTarget);
			}
			else if (isNotEnoughMovement)
			{
				uiManager.Tooltip.SetMessage(TooltipSystem.MessageType.NotEnoughMovement);
			}
			uiManager.Tooltip.EnableMessage(true);
		}
		else
		{
			if (uiManager.Tooltip.IsMessageShowing)
			{
				uiManager.Tooltip.EnableMessage(false);
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