namespace Game.Systems.BattleSystem
{
	public interface IBattlable : IEntity
	{
		public bool InBattle { get; }

		public Battle CurrentBattle { get; }

		public bool JoinBattle(Battle battle);
		public bool LeaveBattle();
	}
}