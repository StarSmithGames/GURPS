using Game.Systems.BattleSystem;
using Game.Systems.SheetSystem;

namespace Game.Systems.CombatDamageSystem
{
	public interface ICombatable : ISheetable, IBattlable, IDamageable, IKillable
	{
		bool CombatWith(IDamageable damageable);
	}
}