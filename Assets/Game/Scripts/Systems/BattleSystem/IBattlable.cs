using Game.Systems.InteractionSystem;

namespace Game.Systems.BattleSystem
{
	public interface IBattlable
	{
		public bool InBattle { get; }
		public bool InAction { get; }

		InteractionPoint OpportunityPoint { get; }

		public BattleExecutor CurrentBattle { get; }

		public bool JoinBattle(BattleExecutor battle);
		public bool LeaveBattle();
	}
}