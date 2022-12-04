using Game.Systems.InteractionSystem;

using UnityEngine;

namespace Game.Systems.CombatDamageSystem
{
	public interface IDamageable : ITransform
	{
		Vector3 DamagePosition { get; }
		InteractionPoint BattlePoint { get; }
		MarkPoint MarkPoint { get; }

		Damage GetDamage();
	}
}