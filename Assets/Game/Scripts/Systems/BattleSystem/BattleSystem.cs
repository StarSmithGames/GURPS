using System.Collections.Generic;

using Zenject;

namespace Game.Systems.BattleSystem
{
	public class BattleSystem
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

		public BattleExecutor CurrentExecutor
		{
			get => currentExecutor;
			private set
			{
				if(currentExecutor != value || currentExecutor == null)
				{
					currentExecutor = value;
				
					signalBus?.Fire(new SignalCurrentBattleExecutorChanged() { currentBattleExecutor = currentExecutor });
				}
			}
		}
		private BattleExecutor currentExecutor;

		private List<BattleExecutor> executors = new List<BattleExecutor>();

		private SignalBus signalBus;
		private BattleExecutor.Factory battleExecutorFactory;

		public BattleSystem(SignalBus signalBus, BattleExecutor.Factory battleExecutorFactory)
		{
			this.signalBus = signalBus;
			this.battleExecutorFactory = battleExecutorFactory;
		}

		public void StartBattle(List<IBattlable> entities)
		{
			var battleExecutor = battleExecutorFactory.Create(new BattleExecutor.Settings() { entities = entities });
			CurrentExecutor = battleExecutor;
			executors.Add(battleExecutor);

			CurrentExecutor.Initialize(CurrentExecutor.Start);
		}

		public void StopBattle()
		{
			CurrentExecutor.Stop();
			CurrentExecutor.Dispose();
			executors.Add(CurrentExecutor);

			CurrentExecutor = null;
		}


		[System.Serializable]
		public class Settings
		{
			public bool isInfinityMoveStat = false;
			public bool isInfinityActionStat = false;
		}
	}
}