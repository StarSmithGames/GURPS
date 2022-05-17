using DG.Tweening;

using Game.Entities;
using Game.Systems.BattleSystem;
using Game.Systems.DialogueSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using System.Collections;
using System.Collections.Generic;
using System.Timers;

using UnityEngine;
using UnityEngine.Events;

namespace Game.Systems.InteractionSystem
{
	public class InteractionSystem
	{
		private DialogueSystem.DialogueSystem dialogueSystem;

		public InteractionSystem(DialogueSystem.DialogueSystem dialogueSystem)
		{
			this.dialogueSystem = dialogueSystem;
		}

		public void Interact(IEntity entity, IInteractable interactable)
		{
			if (entity is IActor initiator && interactable is IActor actor)
			{
				entity.TaskSequence
					.Append(new GoToAction(entity, interactable));
				
				if(interactable is IEntity e)
				{
					entity.TaskSequence
						.Append(new RotateToAction(e, entity.Transform));
				}
				
				entity.TaskSequence
					.Append(() => dialogueSystem.StartDialogue(initiator, actor))
					.Execute();
			}
			else if (interactable is IContainer container)
			{
				if (interactable.IsInRange(entity))//контейнер в области
				{
					entity.TaskSequence
						.Execute(new ContainerInteraction(entity, container));
				}
				else
				{
					entity.TaskSequence
						.Append(new GoToAction(entity, interactable))
						.Append(new ContainerInteraction(entity, container))
						.Execute();
				}
			}
		}

		public void InteractInBattle(IEntity entity, IInteractable interactable)
		{
			if (entity is IBattlable from && interactable is IBattlable to)
			{
				if (!from.InAction)
				{
					if (from.InBattle && to.InBattle)
					{
						if (from.Sheet.Stats.ActionPoints.CurrentValue > 0)
						{
							from.Sheet.Stats.ActionPoints.CurrentValue -= 1;
							if (entity is HumanoidEntity)
							{
								entity.TaskSequence.Execute(new HumanoidAttack(from, to));
							}
							else
							{
								entity.TaskSequence.Execute(new Attack(from, to));
							}
						}
						else
						{
							Debug.LogError("Not Enough actions");
						}
					}
				}
			}
		}
	}



	#region ITask
	/// <summary>
	/// Если currentTask.Status == TaskActionStatus.Cancelled тогда последовательность обрывается.
	/// </summary>
	public class TaskSequence
	{
		public bool IsCanBeBreaked => IsSequenceProcess && ProcessTimeInMilliSeconds >= 250;//1/4 sec

		public bool IsSequenceProcess => sequenceCoroutine != null;
		private Coroutine sequenceCoroutine = null;

		private int ProcessTimeInMilliSeconds = -1;
		private Timer timer;

		private object currentTask = null;
		private List<object> tasks = new List<object>();


		private MonoBehaviour owner;

		public TaskSequence(MonoBehaviour owner)
		{
			this.owner = owner;
		}

		public TaskSequence Append(ITaskAction task)
		{
			tasks.Add(task);
			return this;
		}

		public TaskSequence Append(UnityAction action)
		{
			tasks.Add(action);
			return this;
		}

		private void Dispose()
		{
			tasks.Clear();
			sequenceCoroutine = null;
			currentTask = null;

			timer.Stop();
			ProcessTimeInMilliSeconds = -1;
		}

		public void Execute()
		{
			if (!IsSequenceProcess)
			{
				if(tasks.Count > 0)
				{
					sequenceCoroutine = owner.StartCoroutine(Sequence());
				}
			}
		}

		public void Execute(ITaskAction task)
		{
			if (!IsSequenceProcess)
			{
				if (task != null)
				{
					Append(task).Execute();
				}
			}
		}


		private IEnumerator Sequence()
		{
			StartTimer();

			for (int i = 0; i < tasks.Count; i++)
			{
				currentTask = tasks[i];

				if(currentTask is ITaskAction taskAction)
				{
					yield return taskAction.Implementation();

					if (taskAction.Status == TaskActionStatus.Cancelled)
					{
						Debug.LogError("taskAction.Status == TaskActionStatus.Cancelled, Breaked");
						break;
					}
				}
				else if (currentTask is UnityAction action)
				{
					action?.Invoke();
				}
			}

			Dispose();
		}

		private void StartTimer()
		{
			timer = new Timer();
			timer.Elapsed += new ElapsedEventHandler((o, e) => ProcessTimeInMilliSeconds = e.SignalTime.Millisecond );
			timer.Interval = 100;
			timer.Enabled = true;
		}
	}

	public interface ITaskAction
	{
		TaskActionStatus Status { get; }
		IEnumerator Implementation();
	}
	public abstract class TaskActionBase : ITaskAction
	{
		public TaskActionStatus Status => status;
		protected TaskActionStatus status = TaskActionStatus.Preparing;

		protected IEntity entity;

		public TaskActionBase(IEntity entity)
		{
			this.entity = entity;
		}

		public abstract IEnumerator Implementation();
	}
	public abstract class TaskActionInteraction : TaskActionBase
	{
		protected IInteractable interactable;

		protected TaskActionInteraction(IEntity entity, IInteractable interactable) : base(entity)
		{
			this.interactable = interactable;
		}
	}

	public class GoToAction : TaskActionBase
	{
		private Vector3 destination;

		public GoToAction(IEntity entity, Vector3 destination) : base(entity)
		{
			this.destination = destination;
		}

		public GoToAction(IEntity entity, IInteractable interactable) : base(entity)
		{
			this.destination = interactable.GetIteractionPosition(entity);
		}

		public override IEnumerator Implementation()
		{
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

			if(status != TaskActionStatus.Cancelled)
			{
				status = TaskActionStatus.Done;
			}
		}
	}
	public class RotateToAction : TaskActionBase
	{
		private Vector3 point;
		private float duration;

		public RotateToAction(IEntity entity, Vector3 point, float duration = 0.25f) : base(entity)
		{
			this.point = point;
			this.duration = duration; 
		}
		public RotateToAction(IEntity entity, Transform lookAt, float duration = 0.25f) : base(entity)
		{
			point = lookAt.position;
			this.duration = duration;
		}

		public override IEnumerator Implementation()
		{
			status = TaskActionStatus.Running;

			yield return entity.Controller.RotateAnimatedTo(point, duration);

			status = TaskActionStatus.Done;
		}
	}

	public class Attack : TaskActionInteraction
	{
		public Attack(IBattlable from, IInteractable to) : base(from, to)
		{
			entity.AnimatorControl.onAttackEvent += OnAttacked;
		}

		protected virtual void Dispose()
		{
			if (entity != null)
			{
				entity.AnimatorControl.onAttackEvent -= OnAttacked;
			}
		}

		public override IEnumerator Implementation()
		{
			//if (to is IEntity entity)
			{
				if (!entity.Sheet.Conditions.IsContains<Death>())
				{
					//rotate & animation
					Sequence sequence = DOTween.Sequence();

					sequence
						.Append(entity.Controller.RotateAnimatedTo(entity.Transform.position, 0.25f))
						.AppendCallback(entity.AnimatorControl.Attack);
				}
			}
			//else if (to is IDamegeable)
			//{

			//}

			//wait start and then end attack
			yield return new WaitWhile(() => !entity.AnimatorControl.IsAttackProccess);
			yield return new WaitWhile(() => entity.AnimatorControl.IsAttackProccess);

			Dispose();
		}
		

		protected virtual void CheckDeath(IEntity entity)
		{
			if (entity.Sheet.Stats.HitPoints.CurrentValue == 0)
			{
				if (!entity.Sheet.Conditions.IsContains<Death>())
				{
					if (entity.Sheet.Conditions.Add(new Death()))
					{
						entity.AnimatorControl.Death();
						entity.Kill();

						Debug.LogError($"{entity.MonoBehaviour.gameObject.name} died from {entity.MonoBehaviour.gameObject.name}");
					}
				}
			}
		}

		/// <summary>
		/// from attacked to
		/// </summary>
		protected void OnAttacked()
		{
			if(interactable is IEntity entity)
			{
				//var direction = ((lastInteractable as MonoBehaviour).transform.position - transform.position).normalized;
				entity.AnimatorControl.Hit(Random.Range(0, 2));//animation
				entity.ApplyDamage(entity.GetDamage());

				CheckDeath(entity);
			}
		}

		
	}

	public class HumanoidAttack : Attack
	{
		private HumanoidAnimatorControl control;
		private IEquipment equipment;

		public HumanoidAttack(IBattlable from, IInteractable to) : base(from, to)
		{
			control = (from.AnimatorControl as HumanoidAnimatorControl);
			equipment = (from.Sheet as CharacterSheet).Equipment;

			control.onAttackLeftHand += OnAttackedLeftHand;
			control.onAttackRightHand += OnAttackRightHand;
			control.onAttackKick += OnAttacked;
		}

		protected override void Dispose()
		{
			base.Dispose();

			if(control != null)
			{
				control.onAttackLeftHand -= OnAttackedLeftHand;
				control.onAttackRightHand -= OnAttackRightHand;
				control.onAttackKick -= OnAttacked;
			}
		}

		private void OnAttackedLeftHand()
		{
			if (!equipment.WeaponCurrent.Spare.IsEmpty)
			{
				if (interactable is IEntity entity)
				{
					entity.AnimatorControl.Hit(Random.Range(0, 2));//animation
					entity.ApplyDamage(equipment.WeaponCurrent.Spare.Item.GetItemData<WeaponItemData>().weaponDamage.mainDamage);

					CheckDeath(entity);
				}
			}
			else
			{
				OnAttacked();
			}
		}
		private void OnAttackRightHand()
		{
			if (!equipment.WeaponCurrent.Main.IsEmpty)
			{
				if (interactable is IEntity entity)
				{
					entity.AnimatorControl.Hit(Random.Range(0, 2));//animation
					entity.ApplyDamage(equipment.WeaponCurrent.Main.Item.GetItemData<WeaponItemData>().weaponDamage.mainDamage);

					CheckDeath(entity);
				}
			}
			else
			{
				OnAttacked();
			}
		}
	}


	public class ContainerInteraction : TaskActionInteraction
	{
		public ContainerInteraction(IEntity entity, IContainer container) : base(entity, container) { }

		public override IEnumerator Implementation()
		{
			IContainer container = interactable as IContainer;

			if (!container.IsOpened)
			{
				container.Open();
			}

			while (container.IsOpened)
			{
				if (!interactable.IsInRange(entity))
				{
					container.Close();
				}
				yield return null;
			}
		}
	}


	public enum TaskActionStatus
	{
		Preparing,
		Running,
		Cancelled,
		Done,
	}
	#endregion
}