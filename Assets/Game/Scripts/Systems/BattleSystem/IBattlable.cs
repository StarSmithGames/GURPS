using Game.Entities;

namespace Game.Systems.BattleSystem
{
	public interface IBattlable : IEntity
	{
		public bool InBattle { get; }
		public bool InAction { get; }


		public Battle CurrentBattle { get; }

		public bool JoinBattle(Battle battle);
		public bool LeaveBattle();

		public void Attack(IEntity entity);
	}
}