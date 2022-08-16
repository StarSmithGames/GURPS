using Cinemachine;

using Game.Entities;
using Game.Entities.Models;
using Game.Managers.CharacterManager;
using Game.Managers.InputManager;
using Game.Managers.PartyManager;
using Game.Systems.ContextMenu;
using Game.Systems.InteractionSystem;
using Game.Systems.TooltipSystem;

using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

namespace Game.Systems.CameraSystem
{
	public class CameraVisionLocation : CameraVision
	{
		private ICharacter leader;
		private ICharacterModel leaderModel;

		private PartyManager partyManager;
		private UIManager uiManager;
		private ContextMenuHandler contextMenuHandler;

		public CameraVisionLocation(SignalBus signalBus,
			CinemachineBrain brain,
			InputManager inputManager,
			PartyManager partyManager,
			GlobalSettings settings,
			ContextMenuHandler contextMenuHandler) : base(signalBus, brain, inputManager)
		{
			this.partyManager = partyManager;
			this.settings = settings.cameraVisionLocation;
			this.contextMenuHandler = contextMenuHandler;
		}

		public override void Initialize()
		{
			leader = partyManager.PlayerParty.LeaderParty;
			leaderModel = leader.Model as ICharacterModel;

			base.Initialize();

			signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
		}

		public override void Dispose()
		{
			base.Dispose();
			signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
		}

		public override void Tick()
		{
			base.Tick();

			if (!leader.IsInDialogue)
			{
				TooltipRuler();
			}
		}

		protected override void HandleHover(Vector3 point)
		{
			if (!leader.IsInDialogue)
			{
				if (leaderModel.InBattle)
				{
					if (!leaderModel.IsHasTarget)
					{
						if (CurrentObserve != null)
						{
							if (leaderModel.IsWithRangedWeapon && CurrentObserve != leaderModel)
							{
								//leaderModel.Controller.RotateTo(point);
								leaderModel.Markers.SplineMarker.Path(leaderModel.Outfit.LeftHandPivot.position, point);
								leaderModel.Markers.AdditionalSplineMarker.Path(leaderModel.Outfit.LeftHandPivot.position, point);
							}
							else if (CurrentObserve is IInteractable interactable)
							{
								//leaderModel.SetTarget(interactable.GetIteractionPosition(leader));
							}
						}
						else
						{
							leaderModel.SetTarget(point);

							//if (leader.IsRangeAttackTest)
							//{
							//	leader.Controller.RotateTo(point);
							//}
						}
					}
				}
			}
		}

		protected override void HandleMouseClick(Vector3 point)
		{
			if (!leader.IsInDialogue)
			{
				if (inputManager.IsLeftMouseButtonPressed())
				{
					if (CurrentObserve != null)
					{
						if (leaderModel != CurrentObserve)
						{
							//Interaction
							if (inputManager.IsLeftMouseButtonDown())
							{
								if (CurrentObserve is IInteractable interactable)
								{
									if (leaderModel.InBattle)
									{
										bool isCanReach = (leader.Sheet.Stats.Move.CurrentValue - leaderModel.Navigation.FullPathDistance) >= 0 &&
											leaderModel.Navigation.FullPathDistance != 0 && leader.Sheet.Stats.Move.CurrentValue != 0;

										//if (isCanReach || interactable.IsInRange(leader))
										//{
										//	interactionHandler.InteractInBattle(leader, interactable);
										//}
									}
									else
									{
										//interactionHandler.Interact(leader, interactable);
									}
								}
							}
						}
					}
					else
					{
						//Targeting
						if (IsCanHoldMouse || inputManager.IsLeftMouseButtonDown())
						{
							if (IsMouseHit && !IsUI)
							{
								leaderModel.SetDestination(point);
							}
						}
					}
				}
				else if (inputManager.IsRightMouseButtonPressed())
				{
					//ContextMenu
					if (inputManager.IsRightMouseButtonDown())
					{
						leaderModel.Stop();

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
		}

		protected override void ValidatePath(Vector3 point)
		{
			if (!leader.IsInDialogue)
			{
				float pathDistance = CurrentObserve != null ? leaderModel.Navigation.CurrentNavMeshPathDistance : (float)Math.Round(leaderModel.Navigation.FullPathDistance, 2);

				bool isInvalidTarget = !IsMouseHit || !leaderModel.Navigation.NavMeshAgent.IsPathValid(point);
				bool isOutOfRange = IsMouseHit && !IsUI && leaderModel.InBattle && leaderModel.IsWithRangedWeapon && CurrentObserve != null && !IsPointInLeaderRange(point);
				bool isNotEnoughMovement = IsMouseHit && !IsUI &&
					leaderModel.InBattle && !leaderModel.IsHasTarget &&
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
					//if (uiManager.Tooltip.IsMessageShowing)
					//{
					//	uiManager.Tooltip.EnableMessage(false);
					//}
				}
			}
		}

		protected override void OnHoverObserveChanged()
		{
			if (!leader.IsInDialogue)
			{
				if (leaderModel.InBattle)
				{
					if (CurrentObserve == null || CurrentObserve == leaderModel)
					{
						leaderModel.Markers.SplineMarker.Enable(false);
						leaderModel.Markers.AdditionalSplineMarker.Enable(false);
						leaderModel.Markers.AreaMarker.Enable(false);
					}
					else
					{
						if (leaderModel.IsWithRangedWeapon)
						{
							leaderModel.SetTarget((leaderModel as IPathfinderable).Transform.position);//hide target

							leaderModel.Markers.AdditionalSplineMarker.Enable(true);
							leaderModel.Markers.SplineMarker.Enable(true);

							leaderModel.Markers.AreaMarker.Radius = leaderModel.CharacterRange;
							leaderModel.Markers.AreaMarker.Enable(true);
							leaderModel.Markers.AreaMarker.DrawCircle();
						}
					}
				}
			}
		}

		private void TooltipRuler()
		{
			if (leaderModel.InBattle && !leaderModel.IsHasTarget)
			{
				if (IsMouseHit && !IsUI)
				{
					string text = Math.Round(leaderModel.Navigation.CurrentNavMeshPathDistance, 2) +
						SymbolCollector.METRE.ToString() + "-" +
						Math.Round(leaderModel.Navigation.FullPathDistance, 2) +
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

		private bool IsPointInLeaderRange(Vector3 point)
		{
			return (Vector3.Distance(leaderModel.Outfit.LeftHandPivot.position, point) <= leaderModel.CharacterRange);
		}

		private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
		{
			IsCanHoldMouse = /*signal.leader.InBattle ? false : */settings.isCanHoldMouse;

			if(leader != null)
			{
				leaderModel.Markers.SplineMarker.Enable(false);
				leaderModel.Markers.AdditionalSplineMarker.Enable(false);
				leaderModel.Markers.AreaMarker.Enable(false);
			}

			leader = signal.leader;
			leaderModel = signal.leader.Model as ICharacterModel;
		}
	}
}