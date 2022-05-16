using Cinemachine;

using Game.Entities;
using Game.Managers.CharacterManager;
using Game.Managers.GameManager;
using Game.Managers.InputManager;
using Game.Systems.ContextMenu;
using Game.Systems.InteractionSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;
using Game.Systems.TooltipSystem;

using System;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

using Zenject;

namespace Game.Systems.CameraSystem
{
	public class CameraVision : IInitializable, IDisposable, ITickable
	{
		public bool IsCanHoldMouse { get; private set; }

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

		private Character leader;
		private bool isMouseHit;
		private bool isUI;

		private SignalBus signalBus;
		private CinemachineBrain brain;
		private InputManager inputManager;
		private GameManager gameManager;
		private CharacterManager characterManager;
		private Settings settings;
		private UIManager uiManager;
		private InteractionSystem.InteractionSystem interactionHandler;
		private ContextMenuHandler contextMenuHandler;

		public CameraVision(SignalBus signalBus,
			CinemachineBrain brain,
			InputManager inputManager,
			GameManager gameManager,
			CharacterManager characterManager,
			GlobalSettings settings,
			UIManager uiManager,
			InteractionSystem.InteractionSystem interactionHandler,
			ContextMenuHandler contextMenuHandler)
		{
			this.signalBus = signalBus;
			this.brain = brain;
			this.inputManager = inputManager;
			this.gameManager = gameManager;
			this.characterManager = characterManager;
			this.settings = settings.cameraVision;
			this.uiManager = uiManager;
			this.interactionHandler = interactionHandler;
			this.contextMenuHandler = contextMenuHandler;
		}

		public void Initialize()
		{
			IsCanHoldMouse = settings.isCanHoldMouse;

			signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
			
			leader = characterManager.CurrentParty.LeaderParty;
		}

		public void Dispose()
		{
			signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
		}

		public void Tick()
		{
			RaycastHit hit;
			Ray mouseRay = brain.OutputCamera.ScreenPointToRay(inputManager.GetMousePosition());
			isMouseHit = Physics.Raycast(mouseRay, out hit, settings.raycastLength, settings.raycastLayerMask, QueryTriggerInteraction.Ignore);
			isUI = EventSystem.current.IsPointerOverGameObject();
			Vector3 point = hit.point;

			//Looking
			CurrentObserve = isMouseHit && !isUI ? hit.transform.root.GetComponent<IObservable>() : null;

			HandleHover(point);
			HandleMouseClick(point);

			TooltipRuler();

			ValidatePath(point);
		}

		private void HandleHover(Vector3 point)
		{
			if (leader.InBattle)
			{
				if (!leader.IsHasTarget)
				{
					if (CurrentObserve != null)
					{
						if (leader.IsWithRangedWeapon && CurrentObserve != leader)
						{
							leader.Controller.RotateTo(point);
							leader.Markers.SplineMarker.Path(leader.Outfit.LeftHandPivot.position, point);
							leader.Markers.AdditionalSplineMarker.Path(leader.Outfit.LeftHandPivot.position, point);
						}
						else if (CurrentObserve is IInteractable interactable)
						{
							leader.SetTarget(interactable.GetIteractionPosition(leader));
						}
					}
					else
					{
						leader.SetTarget(point);

						//if (leader.IsRangeAttackTest)
						//{
						//	leader.Controller.RotateTo(point);
						//}
					}
				}
			}
		}
		private void OnHoverObserveChanged()
		{
			if (leader.InBattle)
			{
				if (CurrentObserve == null || CurrentObserve == leader)
				{
					leader.Markers.SplineMarker.Enable(false);
					leader.Markers.AdditionalSplineMarker.Enable(false);
					leader.Markers.AreaMarker.Enable(false);
				}
				else
				{
					if (leader.IsWithRangedWeapon)
					{
						leader.SetTarget(leader.transform.position);//hide target

						leader.Markers.AdditionalSplineMarker.Enable(true); 
						leader.Markers.SplineMarker.Enable(true);

						leader.Markers.AreaMarker.Radius = leader.CharacterRange;
						leader.Markers.AreaMarker.Enable(true);
						leader.Markers.AreaMarker.DrawCircle();
					}
				}
			}
		}

		private void HandleMouseClick(Vector3 point)
		{
			if (inputManager.IsLeftMouseButtonPressed())
			{
				if (CurrentObserve != null)
				{
					if (leader != CurrentObserve)
					{
						//Interaction
						if (inputManager.IsLeftMouseButtonDown())
						{
							if (CurrentObserve is IInteractable interactable)
							{
								if (leader.InBattle)
								{
									if (leader.Navigation.IsCanReach || interactable.IsInRange(leader))
									{
										interactionHandler.InteractInBattle(leader, interactable);
									}
								}
								else
								{
									interactionHandler.Interact(leader, interactable);
								}
							}
						}
					}
				}
				else
				{
					//Targeting
					if (!leader.TaskSequence.IsSequenceProcess || leader.TaskSequence.IsCanBeBreaked)
					{
						if (IsCanHoldMouse || inputManager.IsLeftMouseButtonDown())
						{
							if (isMouseHit && !isUI)
							{
								if (!leader.InBattle)
								{
									leader.SetDestination(point);
								}
								else
								{
									if (!leader.IsHasTarget && leader.Sheet.Stats.Move.CurrentValue >= 0.1f)
									{
										leader.SetDestination(point);
									}
								}
							}
						}
					}
				}
			}
			else
			{
				//ContextMenu
				if (inputManager.IsRightMouseButtonDown())
				{
					leader.Stop();

					if (CurrentObserve != null && leader != CurrentObserve)
					{
						contextMenuHandler.SetTarget(CurrentObserve).Show();
					}
					else
					{
						contextMenuHandler.Hide();
					}
				}
			}
		}


		private void TooltipRuler()
		{
			if (leader.InBattle && !leader.IsHasTarget)
			{
				if (isMouseHit && !isUI)
				{
					string text = Math.Round(leader.Navigation.CurrentNavMeshPathDistance, 2) +
						SymbolCollector.METRE.ToString() + "-" +
						Math.Round(leader.Navigation.FullPathDistance, 2) +
						SymbolCollector.METRE.ToString();
					uiManager.Tooltip.SetRulerText(text);
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
		}

		private void ValidatePath(Vector3 point)
		{
			float pathDistance = CurrentObserve != null ? leader.Navigation.CurrentNavMeshPathDistance : (float)Math.Round(leader.Navigation.FullPathDistance, 2);

			bool isInvalidTarget = !isMouseHit || !leader.Navigation.NavMeshAgent.IsPathValid(point);
			bool isOutOfRange = isMouseHit && !isUI && leader.InBattle && leader.IsWithRangedWeapon && CurrentObserve != null && !IsPointInLeaderRange(point);
			bool isNotEnoughMovement = isMouseHit && !isUI &&
				leader.InBattle && !leader.IsHasTarget &&
				(leader.Sheet.Stats.Move.CurrentValue < pathDistance);

			if (isInvalidTarget || isNotEnoughMovement || isOutOfRange)
			{
				if (isInvalidTarget)
				{
					uiManager.Tooltip.SetMessage(TooltipMessageType.InvalidTarget);
				}
				else if (isOutOfRange)
				{
					uiManager.Tooltip.SetMessage(TooltipMessageType.OutOfRange);
				}
				else if (isNotEnoughMovement)
				{
					uiManager.Tooltip.SetMessage(TooltipMessageType.NotEnoughMovement);
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

		private bool IsPointInLeaderRange(Vector3 point)
		{
			return (Vector3.Distance(leader.Outfit.LeftHandPivot.position, point) <= leader.CharacterRange);
		}

		private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
		{
			IsCanHoldMouse = signal.leader.InBattle ? false : settings.isCanHoldMouse;

			if(leader != null)
			{
				leader.Markers.SplineMarker.Enable(false);
				leader.Markers.AdditionalSplineMarker.Enable(false);
				leader.Markers.AreaMarker.Enable(false);
			}

			leader = signal.leader;
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