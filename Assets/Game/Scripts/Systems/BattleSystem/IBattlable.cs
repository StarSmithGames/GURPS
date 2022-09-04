namespace Game.Systems.BattleSystem
{
	public interface IBattlable
	{
		public bool InBattle { get; }
		public bool InAction { get; }

		public BattleExecutor CurrentBattle { get; }

		public bool JoinBattle(BattleExecutor battle);
		public bool LeaveBattle();
	}
}