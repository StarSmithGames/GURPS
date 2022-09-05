using Game.Systems.InteractionSystem;

using UnityEngine;

namespace Game.Systems.CombatDamageSystem
{
	public interface IDamageable
	{
		Vector3 DamagePosition { get; }
		InteractionPoint BattlePoint { get; }

		Damage GetDamage();
	}
}