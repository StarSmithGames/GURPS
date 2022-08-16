using Cinemachine;

using Game.Entities;
using Game.Entities.Models;
using Game.Managers.CharacterManager;
using Game.Managers.InputManager;
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
		private CharacterModel leader;

		private CharacterManager characterManager;
		private UIManager uiManager;
		private ContextMenuHandler contextMenuHandler;

		public CameraVisionLocation(SignalBus signalBus,
			CinemachineBrain brain,
			InputManager inputManager,
			CharacterManager characterManager,
			GlobalSettings settings,
			ContextMenuHandler contextMenuHandler) : base(signalBus, brain, inputManager)
		{
			this.characterManager = characterManager;
			this.settings = settings.cameraVisionLocation;
			this.contextMenuHandler = contextMenuHandler;
		}

		public override void Initialize()
		{
			base.Initialize();

			//leader = characterManager.CurrentParty.LeaderParty;

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

			return;
			if (!leader.IsInDialogue)
			{
				TooltipRuler();
			}
		}

		protected override void HandleHover(Vector3 point)
		{
			return;
			if (!leader.IsInDialogue)
			{

				if (leader.InBattle)
				{
					if (!leader.IsHasTarget)
					{
						if (CurrentObserve != null)
						{
							if (leader.IsWithRangedWeapon && CurrentObserve != leader)
							{
								//leader.Controller.RotateTo(point);
								leader.Markers.SplineMarker.Path(leader.Outfit.LeftHandPivot.position, point);
								leader.Markers.AdditionalSplineMarker.Path(leader.Outfit.LeftHandPivot.position, point);
							}
							else if (CurrentObserve is IInteractable interactable)
							{
								//leader.SetTarget(interactable.GetIteractionPosition(leader));
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
		}

		protected override void HandleMouseClick(Vector3 point)
		{
			return;

			if (!leader.IsInDialogue)
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
										//bool isCanReach = (leader.Sheet.Stats.Move.CurrentValue - leader.Navigation.FullPathDistance) >= 0 &&
										//	leader.Navigation.FullPathDistance != 0 && leader.Sheet.Stats.Move.CurrentValue != 0;

										//if (isCanReach || interactable.IsInRange(leader))
										{
											//interactionHandler.InteractInBattle(leader, interactable);
										}
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
						if (!leader.TaskSequence.IsSequenceProcess || leader.TaskSequence.IsCanBeBreaked)
						{
							if (IsCanHoldMouse || inputManager.IsLeftMouseButtonDown())
							{
								if (IsMouseHit && !IsUI)
								{
									if (!leader.InBattle)
									{
										leader.SetDestination(point);
									}
									else
									{
										//if (!leader.IsHasTarget && leader.Sheet.Stats.Move.CurrentValue >= 0.1f)
										//{
										//	leader.SetDestination(point);
										//}
									}
								}
							}
						}
					}
				}
				else if (inputManager.IsRightMouseButtonPressed())
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
		}

		protected override void ValidatePath(Vector3 point)
		{
			return;
			if (!leader.IsInDialogue)
			{
				float pathDistance = CurrentObserve != null ? leader.Navigation.CurrentNavMeshPathDistance : (float)Math.Round(leader.Navigation.FullPathDistance, 2);

				bool isInvalidTarget = !IsMouseHit || !leader.Navigation.NavMeshAgent.IsPathValid(point);
				bool isOutOfRange = IsMouseHit && !IsUI && leader.InBattle && leader.IsWithRangedWeapon && CurrentObserve != null && !IsPointInLeaderRange(point);
				//bool isNotEnoughMovement = IsMouseHit && !IsUI &&
				//	leader.InBattle && !leader.IsHasTarget &&
				//	(leader.Sheet.Stats.Move.CurrentValue < pathDistance);

				//if (isInvalidTarget || isNotEnoughMovement || isOutOfRange)
				//{
				//	if (isInvalidTarget)
				//	{
				//		uiManager.Tooltip.SetMessage(TooltipMessageType.InvalidTarget);
				//	}
				//	else if (isOutOfRange)
				//	{
				//		uiManager.Tooltip.SetMessage(TooltipMessageType.OutOfRange);
				//	}
				//	else if (isNotEnoughMovement)
				//	{
				//		uiManager.Tooltip.SetMessage(TooltipMessageType.NotEnoughMovement);
				//	}
				//	uiManager.Tooltip.EnableMessage(true);
				//}
				//else
				//{
				//	if (uiManager.Tooltip.IsMessageShowing)
				//	{
				//		uiManager.Tooltip.EnableMessage(false);
				//	}
				//}
			}
		}

		protected override void OnHoverObserveChanged()
		{
			if (!leader.IsInDialogue)
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
		}

		private void TooltipRuler()
		{
			if (leader.InBattle && !leader.IsHasTarget)
			{
				if (IsMouseHit && !IsUI)
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
	}
}