using System.Collections.Generic;

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

		public BattleExecutor CurrentExecutor { get; private set; }

		private List<BattleExecutor> executors = new List<BattleExecutor>();

		private BattleExecutor.Factory battleExecutorFactory;

		public BattleSystem(BattleExecutor.Factory battleExecutorFactory)
		{
			this.battleExecutorFactory = battleExecutorFactory;
		}

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


		[System.Serializable]
		public class Settings
		{
			public bool isInfinityMoveStat = false;
			public bool isInfinityActionStat = false;
		}
	}
}