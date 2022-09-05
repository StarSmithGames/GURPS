using Game.Systems.BattleSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Systems.CombatDamageSystem
{
	public interface ICombatable : ISheetable, IBattlable, IDamageable, IDieable
	{
		bool CombatWith(IDamageable damageable);
	}
}