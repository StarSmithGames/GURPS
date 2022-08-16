using Game.Entities;

using UnityEngine.Events;

namespace Game.Systems.BattleSystem
{
	public interface IBattlable : IEntityModel
	{
		event UnityAction onBattleChanged;

		public bool InBattle { get; }
		public bool InAction { get; }


		public Battle CurrentBattle { get; }

		public bool JoinBattle(Battle battle);
		public bool LeaveBattle();
	}
}