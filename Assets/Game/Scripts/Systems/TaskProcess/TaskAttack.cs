using Game.Systems.CombatDamageSystem;

namespace Game.Systems
{
	public abstract class TaskAttack<INITIATOR, DAMAGEABLE> : TaskAction
		where INITIATOR : IDamageable
		where DAMAGEABLE : IDamageable
	{
		protected INITIATOR initiator;
		protected DAMAGEABLE damageable;

		public TaskAttack(INITIATOR initiator, DAMAGEABLE damageable)
		{
			this.initiator = initiator;
			this.damageable = damageable;
		}
	}
}