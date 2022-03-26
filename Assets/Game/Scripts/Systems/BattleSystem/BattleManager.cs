using System.Collections.Generic;

using Zenject;

namespace Game.Systems.BattleSystem
{
	public class BattleManager
	{
		public Battle CurrentBattle { get; private set; }

		private List<Battle> currentBattles = new List<Battle>();

		private SignalBus signalBus;

		public BattleManager(SignalBus signalBus)
		{
			this.signalBus = signalBus;
		}

		public void AddBattle(Battle battle)
		{
			currentBattles.Add(battle);

			CurrentBattle = battle;

			signalBus?.Fire(new SignalCurrentBattleChanged() { currentBattle = CurrentBattle });
		}

		public void RemoveBattle(Battle battle)
		{
			currentBattles.Add(battle);

			if(CurrentBattle == battle)
			{
				CurrentBattle = null;

				signalBus?.Fire(new SignalCurrentBattleChanged() { currentBattle = CurrentBattle });
			}
		}

		public void SetBattle(Battle battle)
		{
			if (CurrentBattle != null && CurrentBattle != battle)
			{
				CurrentBattle = battle;

				signalBus?.Fire(new SignalCurrentBattleChanged() { currentBattle = CurrentBattle});
			}
		}
	}
}