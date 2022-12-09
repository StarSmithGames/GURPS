using Game.Managers.GameManager;
using Game.Managers.PartyManager;
using Game.Systems.BattleSystem;
using Game.Systems.CameraSystem;
using Game.UI;

using NodeCanvas.Tasks.Actions;

using System;

using UnityEngine;

using Zenject;

namespace Game.Systems.TooltipSystem
{
	public class TooltipSystem : IInitializable, IDisposable, ITickable
	{
		public bool IsCanShowing { get; private set; }
		public bool IsRulerShowing { get; private set; }
		public bool IsMessageShowing { get; private set; }

		private NavigationSystem.NavigationController leaderNavigation;

		private SignalBus signalBus;
		private UIBattleTooltip battleTooltip;
		private PartyManager partyManager;
		private CameraVisionLocation cameraVision;

		public TooltipSystem(SignalBus signalBus, UIBattleTooltip battleTooltip,
			PartyManager partyManager,
			CameraVisionLocation cameraVision)
		{
			this.signalBus = signalBus;
			this.battleTooltip = battleTooltip;
			this.partyManager = partyManager;
			this.cameraVision = cameraVision;
		}

		public void Initialize()
		{
			signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
			signalBus?.Subscribe<SignalGameStateChanged>(OnGameStateChanged);

			EnableRuler(false);
			EnableMessage(false);
		}

		public void Dispose()
		{
			signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
			signalBus?.Unsubscribe<SignalGameStateChanged>(OnGameStateChanged);
		}

		public void Tick()
		{
			if(cameraVision.IsMouseHit && !cameraVision.IsUI)
			{
				if (IsRulerShowing)
				{
					battleTooltip.Ruler.text =
						Math.Round(leaderNavigation.CurrentPath.Distance, 2) + SymbolCollector.METRE.ToString() + "-" +
						Math.Round(leaderNavigation.FullPath.Distance, 2) + SymbolCollector.METRE.ToString(); ;
				}

				IsCanShowing = true;
			}
			else
			{
				if(IsCanShowing != false)
				{
					SetMessage(TooltipMessageType.None);
					battleTooltip.Ruler.text = "";
				}

				IsCanShowing = false;
			}

			//Move
			if (IsRulerShowing || IsMessageShowing)
			{
				Vector2 position = Input.mousePosition;
				position += OffsetRightDown(battleTooltip.Tooltip);

				battleTooltip.Tooltip.anchoredPosition = position;
			}

			//if (!leaderModel.IsInDialogue)
			//{
			//	ValidatePath(HitPoint);
			//}

			//if (leaderModel.InBattle && !leaderModel.IsHasTarget)
			//{
			//	if (battleSystem.CurrentExecutor.CurrentInitiator == leaderModel)
			//	{
			//		if (battleSystem.CurrentExecutor.CurrentState == BattleExecutorState.Battle)
			//		{
			//			TooltipRuler();
			//		}
			//	}
			//}
		}

		//private void ValidatePath(Vector3 point)
		//{
		//	float pathDistance = CurrentObserve != null ? leaderModel.Navigation.CurrentNavMeshPathDistance : (float)Math.Round(leaderModel.Navigation.FullPath.Distance, 2);

		//	bool isInvalidTarget = !IsMouseHit || !leaderModel.Navigation.NavMeshAgent.IsPathValid(point);
		//	bool isOutOfRange = IsMouseHit && !IsUI && leaderModel.InBattle && leaderModel.IsWithRangedWeapon && CurrentObserve != null && !IsPointInLeaderRange(point);
		//	bool isNotEnoughMovement = IsMouseHit && !IsUI &&
		//		leaderModel.InBattle && !leaderModel.IsHasTarget &&
		//		(leader.Sheet.Stats.Move.CurrentValue < pathDistance);

		//	bool isError = isInvalidTarget || isNotEnoughMovement || isOutOfRange;

		//	if (isError)
		//	{
		//		if (isInvalidTarget)
		//		{
		//			tooltipSystem.SetMessage(TooltipMessageType.InvalidTarget);
		//		}
		//		else if (isOutOfRange)
		//		{
		//			tooltipSystem.SetMessage(TooltipMessageType.OutOfRange);
		//		}
		//		else if (isNotEnoughMovement)
		//		{
		//			tooltipSystem.SetMessage(TooltipMessageType.NotEnoughMovement);
		//		}
		//		tooltipSystem.EnableMessage(true);
		//	}
		//	else
		//	{
		//		if (tooltipSystem.IsMessageShowing)
		//		{
		//			tooltipSystem.EnableMessage(false);
		//		}
		//	}
		//}


		public void EnableRuler(bool trigger)
		{
			if (trigger)
			{
				battleTooltip.transform.SetAsLastSibling();
			}
			IsRulerShowing = trigger;
			battleTooltip.Ruler.gameObject.SetActive(trigger);
		}

		public void EnableMessage(bool trigger)
		{
			if (trigger)
			{
				battleTooltip.transform.SetAsLastSibling();
			}
			IsMessageShowing = trigger;
			battleTooltip.Message.gameObject.SetActive(trigger);
		}

		public void SetMessage(TooltipMessageType type)
		{
			switch (type)
			{
				case TooltipMessageType.InvalidTarget:
				{
					battleTooltip.Message.text = "Invalid Target";
					battleTooltip.Message.color = Color.red;
					break;
				}
				case TooltipMessageType.NotEnoughMovement:
				{
					battleTooltip.Message.text = "Not Enough Movement";
					battleTooltip.Message.color = Color.yellow;
					break;
				}
				case TooltipMessageType.CanNotReachDesination:
				{
					battleTooltip.Message.text = "Can't Reach Destination";
					battleTooltip.Message.color = Color.white;
					break;
				}
				case TooltipMessageType.OutOfRange:
				{
					battleTooltip.Message.text = "OutOfRange";
					battleTooltip.Message.color = Color.red;
					break;
				}
				default:
				{
					battleTooltip.Message.text = "";
					break;
				}
			}
		}

		private Vector2 OffsetRightDown(RectTransform rectTransform) => new Vector2((rectTransform.sizeDelta.x / 2) * 1.5f, -(rectTransform.sizeDelta.y / 2) * 2.5f);
	
		private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
		{
			leaderNavigation = signal.leader.Model.Navigation;
		}

		private void OnGameStateChanged(SignalGameStateChanged signal)
		{
			if(signal.newGameState == GameState.Gameplay)
			{
				leaderNavigation = partyManager.PlayerParty.LeaderParty.Model.Navigation;
			}
		}
	}

	public enum TooltipMessageType
	{
		None,
		InvalidTarget,
		NotEnoughMovement,
		CanNotReachDesination,
		OutOfRange,
	}
}