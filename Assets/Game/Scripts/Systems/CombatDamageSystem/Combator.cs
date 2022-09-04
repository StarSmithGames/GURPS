namespace Game.Systems.CombatDamageSystem
{
	public static class Combator
	{
		public static void ABCombat(ICombatable initiator, IDamageable damageable)
		{
			initiator.CombatWith(damageable);
		}
	}
}