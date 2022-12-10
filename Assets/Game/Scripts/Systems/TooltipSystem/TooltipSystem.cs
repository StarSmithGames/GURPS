using Game.Systems.CameraSystem;

using UnityEngine;

using Zenject;

namespace Game.Systems.TooltipSystem
{
	public class TooltipSystem : IInitializable, ITickable
	{
		public bool IsCanShowing { get; private set; }
		public bool IsMessageShowing { get; private set; }

		private SignalBus signalBus;
		private UIObjectTooltip objectTooltip;
		private UIBattleTooltip battleTooltip;
		private CameraVisionLocation cameraVision;

		public TooltipSystem(SignalBus signalBus,
			UIObjectTooltip objectTooltip,
			UIBattleTooltip battleTooltip,
			CameraVisionLocation cameraVision)
		{
			this.signalBus = signalBus;
			this.objectTooltip = objectTooltip;
			this.battleTooltip = battleTooltip;
			this.cameraVision = cameraVision;
		}

		public void Initialize()
		{
			DisableBattleTooltip();
		}

		public void Tick()
		{
			if(cameraVision.IsMouseHit && !cameraVision.IsUI)
			{
				IsCanShowing = true;
			}
			else
			{
				if(IsCanShowing != false)
				{
					DisableBattleTooltip();
				}

				IsCanShowing = false;
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

		public void SetRuller(TooltipRulerType rulerType)
		{
			battleTooltip.Ruller.SetType(rulerType);
		}

		public void SetRullerChance(string chance)
		{
			battleTooltip.Ruller.SetType(TooltipRulerType.Chance);
			battleTooltip.Ruller.Ruler.text = chance;
		}

		public void SetRullerPath(NavigationSystem.NavigationPath path)
		{
			battleTooltip.Ruller.SetCustomPath(path);
		}

		public void SetMessage(TooltipMessageType messageType)
		{
			battleTooltip.Message.SetMessage(messageType);
		}

		public void SetMessage(string add, TooltipAdditionalMessageType messageType)
		{
			battleTooltip.AdditionalMessage.SetMessage(add, messageType);
		}

		private void DisableBattleTooltip()
		{
			SetRuller(TooltipRulerType.None);
			SetMessage(TooltipMessageType.None);
			SetMessage("", TooltipAdditionalMessageType.None);
		}
	}
}