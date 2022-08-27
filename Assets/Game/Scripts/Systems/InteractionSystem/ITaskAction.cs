
using Game.Entities.Models;

using System.Collections;
using System.Collections.Generic;
using System.Timers;

using UnityEngine.Events;

using UnityEngine;

namespace Game.Systems.InteractionSystem
{
	public interface ITaskAction
	{
		TaskActionStatus Status { get; }
		IEnumerator Implementation();
	}

	public abstract class TaskActionBase : ITaskAction
	{
		public TaskActionStatus Status => status;
		protected TaskActionStatus status = TaskActionStatus.Preparing;

		protected IEntityModel entity;

		public TaskActionBase(IEntityModel entity)
		{
			this.entity = entity;
		}

		public abstract IEnumerator Implementation();
	}
	public abstract class TaskActionInteraction : TaskActionBase
	{
		protected IInteractable interactable;

		protected TaskActionInteraction(IEntityModel entity, IInteractable interactable) : base(entity)
		{
			this.interactable = interactable;
		}
	}

	public class GoToAction : TaskActionBase
	{
		private Vector3 destination;

		public GoToAction(IEntityModel entity, Vector3 destination) : base(entity)
		{
			this.destination = destination;
		}

		public override IEnumerator Implementation()
		{
			status = TaskActionStatus.Preparing;
			entity.SetDestination(destination);

			Vector3 lastDestination = entity.Navigation.CurrentNavMeshDestination;

			status = TaskActionStatus.Running;

			yield return new WaitWhile(() =>
			{
				if (lastDestination != entity.Navigation.CurrentNavMeshDestination)
				{
					status = TaskActionStatus.Cancelled;
					return false;
				}
				return !entity.Navigation.NavMeshAgent.IsReachedDestination();
			});

			if (status != TaskActionStatus.Cancelled)
			{
				status = TaskActionStatus.Done;
			}
		}
	}

	public class RotateToAction : TaskActionBase
	{
		private Vector3 point;
		private float duration;

		public RotateToAction(IEntityModel entity, Vector3 point, float duration = 0.25f) : base(entity)
		{
			this.point = point;
			this.duration = duration;
		}
		public RotateToAction(IEntityModel entity, Transform lookAt, float duration = 0.25f) : base(entity)
		{
			point = lookAt.position;
			this.duration = duration;
		}

		public override IEnumerator Implementation()
		{
			status = TaskActionStatus.Running;

			//yield return entity.Controller.RotateAnimatedTo(point, duration);
			yield return null;
			status = TaskActionStatus.Done;
		}
	}

	//public class Attack : TaskActionInteraction
	//{
	//	protected virtual void Dispose()
	//	{
	//		if (entity != null)
	//		{
	//			//entity.AnimatorControl.onAttackEvent -= OnAttacked;
	//		}
	//	}

	//	public override IEnumerator Implementation()
	//	{
	//		//if (to is IEntity entity)
	//		{
	//			//if (!entity.Sheet.Conditions.IsContains<Death>())
	//			{
	//				//rotate & animation
	//				Sequence sequence = DOTween.Sequence();

	//				//sequence
	//				//	.Append(entity.Controller.RotateAnimatedTo(entity.Transform.position, 0.25f))
	//				//	.AppendCallback(entity.AnimatorControl.Attack);
	//			}
	//		}
	//		//else if (to is IDamegeable)
	//		//{

	//		//}

	//		//wait start and then end attack
	//		//yield return new WaitWhile(() => !entity.AnimatorControl.IsAttackProccess);
	//		//yield return new WaitWhile(() => entity.AnimatorControl.IsAttackProccess);
	//		yield return null;
	//		Dispose();
	//	}


	//	protected virtual void CheckDeath(IEntityModel entity)
	//	{
	//		//if (entity.Sheet.Stats.HitPoints.CurrentValue == 0)
	//		//{
	//		//	if (!entity.Sheet.Conditions.IsContains<Death>())
	//		//	{
	//		//		if (entity.Sheet.Conditions.Add(new Death()))
	//		//		{
	//		//			//entity.AnimatorControl.Death();
	//		//			//entity.Kill();

	//		//			Debug.LogError($"{entity.MonoBehaviour.gameObject.name} died from {entity.MonoBehaviour.gameObject.name}");
	//		//		}
	//		//	}
	//		//}
	//	}

	//	/// <summary>
	//	/// from attacked to
	//	/// </summary>
	//	protected void OnAttacked()
	//	{
	//		if(interactable is IEntityModel entity)
	//		{
	//			//var direction = ((lastInteractable as MonoBehaviour).transform.position - transform.position).normalized;
	//			//entity.AnimatorControl.Hit(Random.Range(0, 2));//animation
	//			//entity.ApplyDamage(entity.GetDamage());

	//			//CheckDeath(entity);
	//		}
	//	}


	//}

	//public class HumanoidAttack : Attack
	//{
	//	private HumanoidAnimatorControl control;
	//	private IEquipment equipment;

	//	public HumanoidAttack(IBattlable from, IInteractable to) : base(from, to)
	//	{
	//		//control = (from.AnimatorControl as HumanoidAnimatorControl);
	//		//equipment = (from.Sheet as CharacterSheet).Equipment;

	//		//control.onAttackLeftHand += OnAttackedLeftHand;
	//		//control.onAttackRightHand += OnAttackRightHand;
	//		//control.onAttackKick += OnAttacked;
	//	}

	//	protected override void Dispose()
	//	{
	//		base.Dispose();

	//		if(control != null)
	//		{
	//			control.onAttackLeftHand -= OnAttackedLeftHand;
	//			control.onAttackRightHand -= OnAttackRightHand;
	//			control.onAttackKick -= OnAttacked;
	//		}
	//	}

	//	private void OnAttackedLeftHand()
	//	{
	//		if (!equipment.WeaponCurrent.Spare.IsEmpty)
	//		{
	//			if (interactable is IEntityModel entity)
	//			{
	//				//entity.AnimatorControl.Hit(Random.Range(0, 2));//animation
	//				//entity.ApplyDamage(equipment.WeaponCurrent.Spare.Item.GetItemData<WeaponItemData>().weaponDamage.mainDamage);

	//				//CheckDeath(entity);
	//			}
	//		}
	//		else
	//		{
	//			OnAttacked();
	//		}
	//	}
	//	private void OnAttackRightHand()
	//	{
	//		if (!equipment.WeaponCurrent.Main.IsEmpty)
	//		{
	//			if (interactable is IEntityModel entity)
	//			{
	//				//entity.AnimatorControl.Hit(Random.Range(0, 2));//animation
	//				//entity.ApplyDamage(equipment.WeaponCurrent.Main.Item.GetItemData<WeaponItemData>().weaponDamage.mainDamage);

	//				//CheckDeath(entity);
	//			}
	//		}
	//		else
	//		{
	//			OnAttacked();
	//		}
	//	}
	//}

	public enum TaskActionStatus
	{
		Preparing,
		Running,
		Cancelled,
		Done,
	}
}