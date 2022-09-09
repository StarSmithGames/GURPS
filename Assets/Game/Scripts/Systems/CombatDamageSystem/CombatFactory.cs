using Game.Entities.Models;
using Game.Systems.AnimatorController;
using Zenject;

namespace Game.Systems.CombatDamageSystem
{
	public interface ICombat
	{
		void AttackAnimation();
		void DealDamage();
	}

	public class CombatBase : ICombat
	{
		protected ICharacterModel initiator;
		protected IDamageable damageable;

		protected CombatDamageSystem combatDamageSystem;

		public CombatBase(ICharacterModel initiator, IDamageable damageable,
			CombatDamageSystem combatDamageSystem)
		{
			this.initiator = initiator;
			this.damageable = damageable;
			this.combatDamageSystem = combatDamageSystem;
		}

		public virtual void AttackAnimation()
		{
			initiator.AnimatorController.Attack();
		}

		public virtual void DealDamage()
		{
			combatDamageSystem.DealDamage(initiator.GetDamage(), damageable);
		}

		public class Factory : PlaceholderFactory<ICharacterModel, IDamageable, CombatBase> { }
	}

	public class CombatHumanoid : CombatBase
	{
		private HumanoidAnimatorController animatorController;

		public CombatHumanoid(ICharacterModel initiator, IDamageable damageable, CombatDamageSystem combatDamageSystem)
			: base(initiator, damageable, combatDamageSystem)
		{
			animatorController = initiator.AnimatorController as HumanoidAnimatorController;
		}

		public override void AttackAnimation()
		{
			animatorController.AttackKick();
		}

		public override void DealDamage()
		{
			combatDamageSystem.DealDamage(initiator.GetDamage(), damageable);
		}

		public new class Factory : PlaceholderFactory<ICharacterModel, IDamageable, CombatHumanoid> { }
	}

	public class CombatFactory : PlaceholderFactory<ICharacterModel, IDamageable, ICombat> { }

	public class CustomCombatFactory : IFactory<ICharacterModel, IDamageable, ICombat>
	{
		private CombatBase.Factory combatFactory;
		private CombatHumanoid.Factory humanoidCombatFactory;

		public CustomCombatFactory(CombatBase.Factory combatFactory,
			CombatHumanoid.Factory humanoidCombatFactory)
		{
			this.combatFactory = combatFactory;
			this.humanoidCombatFactory = humanoidCombatFactory;
		}

		public ICombat Create(ICharacterModel model, IDamageable damageable)
		{
			if (model is HumanoidCharacterModel)
			{
				return humanoidCombatFactory.Create(model, damageable);
			}

			return combatFactory.Create(model, damageable);
		}
	}
}