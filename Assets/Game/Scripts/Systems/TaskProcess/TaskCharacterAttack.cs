using Game.Entities.Models;
using Game.Systems.AnimatorController;
using Game.Systems.CombatDamageSystem;

using System.Collections;

using UnityEngine;

using Zenject;

namespace Game.Systems
{
	public abstract class TaskCharacterAttack<DAMAGEABLE> : TaskAttack<ICharacterModel, DAMAGEABLE>
		where DAMAGEABLE : IDamageable
	{
		protected HumanoidAnimatorController initiatorAnimator;

		protected CombatDamageSystem.CombatDamageSystem combatDamageSystem;

		public TaskCharacterAttack(ICharacterModel initiator, DAMAGEABLE damageable,
			CombatDamageSystem.CombatDamageSystem combatDamageSystem)
			: base(initiator, damageable)
		{
			this.combatDamageSystem = combatDamageSystem;
		}

		protected virtual void Initialization()
		{
			initiatorAnimator = initiator.AnimatorController as HumanoidAnimatorController;
			initiatorAnimator.onAttackEvent += OnAttacked;
			initiatorAnimator.onAttackLeftHand += OnAttackedLeftHand;
			initiatorAnimator.onAttackRightHand += OnAttackRightHand;
			initiatorAnimator.onAttackKick += OnAttackKick;
		}

		protected virtual void Dispose()
		{
			if (initiatorAnimator != null)
			{
				initiatorAnimator.onAttackEvent -= OnAttacked;
				initiatorAnimator.onAttackLeftHand -= OnAttackedLeftHand;
				initiatorAnimator.onAttackRightHand -= OnAttackRightHand;
				initiatorAnimator.onAttackKick -= OnAttackKick;
			}
		}

		public override IEnumerator Implementation()
		{
			//if (to is IEntity entity)
			{
				//if (!entity.Sheet.Conditions.IsContains<Death>())
				{
					//rotate & animation
					//Sequence sequence = DOTween.Sequence();

					//sequence
					//	.Append(entity.Controller.RotateAnimatedTo(entity.Transform.position, 0.25f))
					//	.AppendCallback(entity.AnimatorControl.Attack);
				}
			}
			//else if (to is IDamegeable)
			//{

			//}
			Initialization();
			yield return null;
			Attack();

			//wait start and then end attack
			yield return new WaitUntil(() => initiator.AnimatorController.IsAttackProccess);
			yield return new WaitUntil(() => !initiator.AnimatorController.IsAttackProccess);
			Dispose();
		}

		protected virtual void Attack()
		{
			initiator.AnimatorController.Attack();
		}

		protected virtual void OnAttacked()
		{
			Debug.LogError("OnAttacked");
		}

		protected virtual void OnAttackedLeftHand()
		{
			Debug.LogError("OnAttackedLeftHand");
			OnAttacked();
		}

		protected virtual void OnAttackRightHand()
		{
			Debug.LogError("OnAttackRightHand");
			OnAttacked();
		}

		protected virtual void OnAttackKick()
		{
			Debug.LogError("OnAttackKick");
			OnAttacked();
		}
	}

	public class TaskCharacterAttackDamageable : TaskCharacterAttack<IDamageable>
	{
		public TaskCharacterAttackDamageable(ICharacterModel initiator, IDamageable damageable,
			CombatDamageSystem.CombatDamageSystem combatDamageSystem)
			: base(initiator, damageable, combatDamageSystem) { }

		protected override void Attack()
		{
			initiator.AnimatorController.Attack();
		}

		//protected override void Hit()
		//{
		//	//var direction = ((lastInteractable as MonoBehaviour).transform.position - transform.position).normalized;
		//}

		protected override void OnAttacked()
		{
			combatDamageSystem.DealDamage(initiator.GetDamage(), damageable);
		}

		public class Factory : PlaceholderFactory<ICharacterModel, IDamageable, TaskCharacterAttackDamageable> { }
	}

	public class TaskCharacterAttackCharacter : TaskCharacterAttack<ICharacterModel>
	{
		public TaskCharacterAttackCharacter(ICharacterModel initiator, ICharacterModel characterModel,
			CombatDamageSystem.CombatDamageSystem combatDamageSystem)
			: base(initiator, characterModel, combatDamageSystem) { }

		protected override void OnAttacked()
		{
			combatDamageSystem.DealDamage(initiator.GetDamage(), damageable);
			//damageable.AnimatorController.Hit(Random.Range(0, 2));//animation
		}

		public class Factory : PlaceholderFactory<ICharacterModel, ICharacterModel, TaskCharacterAttackCharacter> { }
	}

	//Factories
	public class CustomCharacterAttackFactory : IFactory<ICharacterModel, IDamageable, ITaskAction>
	{
		private TaskCharacterAttackCharacter.Factory characterAttackCharacterFactory;
		private TaskCharacterAttackDamageable.Factory characterAttackDamageableFactory;

		public CustomCharacterAttackFactory(TaskCharacterAttackCharacter.Factory characterAttackCharacterFactory,
			TaskCharacterAttackDamageable.Factory characterAttackDamageableFactory)
		{
			this.characterAttackCharacterFactory = characterAttackCharacterFactory;
			this.characterAttackDamageableFactory = characterAttackDamageableFactory;
		}

		public ITaskAction Create(ICharacterModel characterModel, IDamageable damageable)
		{
			if (damageable is ICharacterModel model)
			{
				return characterAttackCharacterFactory.Create(characterModel, model);
			}
			else
			{
				return characterAttackDamageableFactory.Create(characterModel, damageable);
			}
		}
	}

	public class CharacterAttackFactory : PlaceholderFactory<ICharacterModel, IDamageable, ITaskAction> { }
}