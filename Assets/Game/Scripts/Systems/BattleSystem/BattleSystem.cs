using Game.Managers.CharacterManager;
using DG.Tweening;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Game.Systems.CommandCenter;

namespace Game.Systems.BattleSystem
{
	//Turn Based BattleSystem
	public partial class BattleSystem
	{
		public BattleManager BattleManager
		{
			get
			{
				if(battleManager == null)
				{
					battleManager = new BattleManager();
				}

				return battleManager;
			}
		}
		private BattleManager battleManager;

		public IBattlable CurrentInitiator { get; private set; }

		public BattleExecutor CurrentExecutor { get; private set; }

		private List<BattleExecutor> executors = new List<BattleExecutor>();


		private BattleExecutor.Factory battleExecutorFactory;

		public BattleSystem(BattleExecutor.Factory battleExecutorFactory)
		{
			this.battleExecutorFactory = battleExecutorFactory;
		}

		private Battle localBattleTest;

		public void StartBattle(List<IBattlable> entities)
		{
			var battleExecutor = battleExecutorFactory.Create(new BattleExecutor.Settings() { entities = entities });
			CurrentExecutor = battleExecutor;
			executors.Add(battleExecutor);

			CurrentExecutor.Initialize();
			CurrentExecutor.Start();
		}

		public void StopBattle()
		{
			CurrentExecutor.Stop();
			CurrentExecutor.Dispose();
			executors.Add(CurrentExecutor);
		}

		private void UpdateInitiatorTurn()
		{
			//IEntityModel initiator = localBattleTest.BattleFSM.CurrentTurn.Initiator;
			//bool isMineTurn = characterManager.CurrentParty.Characters.Contains(initiator);

			//cameraController.LookAt(initiator);

			//uiManager.Battle.Messages.ShowTurnInforamtion(isMineTurn ? "YOU TURN" : "ENEMY TURN");

			//uiManager.Battle.SkipTurn.Enable(isMineTurn);
			//uiManager.Battle.RunAway.Enable(isMineTurn);
		}


		#region Stats Move, ActionsPoints 


		private void InitiatorRecoveMove(bool isAnimated = true)
		{
			//IStatBar stat = CurrentInitiator.Sheet.Stats.Move;

			//if (stat.CurrentValue < stat.MaxValue)
			//{
			//	if (isAnimated)
			//	{
			//		float from = stat.CurrentValue;
			//		float to = stat.MaxValue;
				
			//		DOTween.To(() => from, (x) => stat.CurrentValue = x, to, 0.5f);
			//	}
			//	else
			//	{
			//		stat.CurrentValue = stat.MaxValue;
			//	}
			//}
		}

		private void InitiatorRecoveActionsPoints()
		{
			//var stat = CurrentInitiator.Sheet.Stats.ActionPoints;
			//stat.CurrentValue = stat.MaxValue;

		}
		#endregion
		




		[System.Serializable]
		public class Settings
		{
			public bool isInfinityMoveStat = false;
			public bool isInfinityActionStat = false;
		}
	}
}