using Game.Systems.InteractionSystem;

using UnityEngine.Events;

namespace Game.Systems.BattleSystem
{
	public interface IBattlable
	{
		event UnityAction onBattleChanged;

		public bool InBattle { get; }
		public bool InAction { get; }

		InteractionPoint OpportunityPoint { get; }

		public BattleExecutor CurrentBattle { get; }

		public bool JoinBattle(BattleExecutor battle);
		public bool LeaveBattle();
	}
}