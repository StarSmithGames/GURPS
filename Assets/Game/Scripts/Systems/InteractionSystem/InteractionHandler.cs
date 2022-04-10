using DG.Tweening;

using Game.Entities;
using Game.Systems.BattleSystem;
using Game.Systems.CameraSystem;
using Game.Systems.DamageSystem;
using Game.Systems.FloatingTextSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using System.Collections;

using UnityEngine;

namespace Game.Systems.InteractionSystem
{
	public class InteractionHandler
	{
		private bool terminate = false;
		private IEntity entity;
		private IInteractable interactable;

		public bool IsInteractionProcess => interactionCoroutine != null;
		private Coroutine interactionCoroutine = null;

		private AsyncManager asyncManager;

		public InteractionHandler(AsyncManager asyncManager)
		{
			this.asyncManager = asyncManager;
		}

		public void Interact(IEntity entity, IInteractable interactable)
		{
			if (IsInteractionProcess) return;

			this.entity = entity;
			this.interactable = interactable;

			entity.LastInteractionAction = null;

			if (entity is IBattlable from && interactable is IBattlable to)
			{
				if (from.InBattle && to.InBattle)
				{
					if (from.Sheet.Stats.ActionPoints.CurrentValue > 0)
					{
						from.Sheet.Stats.ActionPoints.CurrentValue -= 1;
						entity.LastInteractionAction = new Attack(from, to);
					}
					else
					{
						Debug.LogError("Not Enough actions");
					}
				}
			}
			else
			{
				entity.LastInteractionAction = new BaseInteraction(entity, interactable);
			}

			if(entity.LastInteractionAction != null)
			{
				interactionCoroutine = asyncManager.StartCoroutine(Interaction(entity.LastInteractionAction));
			}
		}

		private IEnumerator Interaction(IAction action)
		{
			yield return PreInteraction();
			if (terminate)
			{
				terminate = false;
				interactionCoroutine = null;
				yield break;
			}
			yield return action.Interaction();

			interactionCoroutine = null;
		}

		private IEnumerator PreInteraction()
		{
			if (!interactable.IsInRange(entity))
			{
				entity.SetDestination(interactable.GetIteractionPosition(entity));

				Vector3 lastDestination = entity.Navigation.CurrentNavMeshDestination;

				yield return new WaitWhile(() =>
				{
					if (lastDestination != entity.Navigation.CurrentNavMeshDestination)
					{
						terminate = true;
						return false;
					}
					return !entity.Navigation.NavMeshAgent.IsReachedDestination();
				});
			}
		}
	}



	#region IAction
	public interface IAction
	{
		IEnumerator Interaction();
	}

	public class Attack : IAction
	{
		private IBattlable from;
		private IDamegeable to;

		public Attack(IBattlable from, IDamegeable to)
		{
			this.from = from;
			this.to = to;

			var control = (from.AnimatorControl as HumanoidAnimatorControl);

			control.onAttackEvent += OnAttacked;

			control.onAttackLeftHand += OnAttacked;
			control.onAttackRightHand += OnAttacked;
			control.onAttackKick += OnAttacked;
		}

		private void Dispose()
		{
			var control = (from.AnimatorControl as HumanoidAnimatorControl);

			if (control != null)
			{
				control.onAttackEvent -= OnAttacked;

				control.onAttackLeftHand -= OnAttacked;
				control.onAttackRightHand -= OnAttacked;
				control.onAttackKick -= OnAttacked;
			}
		}

		public IEnumerator Interaction()
		{
			if (to is IEntity entity)
			{
				if (!entity.Sheet.Conditions.IsContains<Death>())
				{
					//rotate & animation
					Sequence sequence = DOTween.Sequence();

					sequence
						.Append(from.Controller.RotateAnimatedTo(entity.Transform.position, 0.25f))
						.AppendCallback(from.Attack);
				}
			}
			else if (to is IDamegeable)
			{

			}

			//wait start and then end attack
			var anim = from.AnimatorControl as HumanoidAnimatorControl;

			yield return new WaitWhile(() => !anim.IsAttackProccess);
			yield return new WaitWhile(() => anim.IsAttackProccess);

			Dispose();
		}

		/// <summary>
		/// from attacked to
		/// </summary>
		private void OnAttacked()
		{
			if(to is IEntity entity)
			{
				Debug.LogError("Hit");

				//var direction = ((lastInteractable as MonoBehaviour).transform.position - transform.position).normalized;
				entity.AnimatorControl.Hit(Random.Range(0, 2));//animation
				entity.ApplyDamage(from.GetDamage());

				if (entity.Sheet.Stats.HitPoints.CurrentValue == 0)
				{
					if (!entity.Sheet.Conditions.IsContains<Death>())
					{
						if (entity.Sheet.Conditions.Add(new Death()))
						{
							entity.AnimatorControl.Death();
							entity.Kill();

							Debug.LogError($"{entity.MonoBehaviour.gameObject.name} died from {from.MonoBehaviour.gameObject.name}");
						}
					}
				}
			}
		}
	}

	public class BaseInteraction : IAction
	{
		private IEntity entity;
		private IInteractable interactable;

		public BaseInteraction(IEntity entity, IInteractable interactable)
		{
			this.entity = entity;
			this.interactable = interactable;
		}

		public IEnumerator Interaction()
		{
			if (interactable is IContainer container)
			{
				container.Open();

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
	}
	#endregion
}