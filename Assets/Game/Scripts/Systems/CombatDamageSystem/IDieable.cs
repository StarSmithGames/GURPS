using UnityEngine.Events;

namespace Game.Systems.CombatDamageSystem
{
	public interface IDieable
	{
		event UnityAction<IDieable> onDied;

		void Die();
	}
}