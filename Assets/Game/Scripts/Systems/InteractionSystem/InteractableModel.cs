using EPOOutline;

using Game.Entities;
using Game.Systems.BattleSystem;

using Sirenix.OdinInspector;

using System.Collections;

using UnityEngine;

namespace Game.Systems.InteractionSystem
{
	public class InteractableModel : MonoBehaviour, IInteractable, IObservable
	{
		[SerializeField] protected Settings interactableSettings;
		[Space]
		[SerializeField] protected Outlinable outline;

		public bool IsInteractable => outline.enabled;

		protected IInteractable lastInteractable = null;
		protected IEntity currentInteractor = null;

		private void Awake()
		{
			outline.enabled = false;
		}

		public Vector3 GetIteractionPosition(IEntity entity = null)
		{
			if (interactableSettings.interaction == InteractionType.CustomPoint)
			{
				return transform.TransformPoint(interactableSettings.position);
			}
			else
			{
				if (entity != null)
				{
					if (IsInteractorInRange(entity)) return entity.Transform.position;

					return transform.position + ((interactableSettings.maxRange - 0.1f) * (entity.Transform.position - transform.position).normalized);
				}
			}

			return transform.position;
		}

		public bool IsInteractorInRange(IEntity entity)
		{
			if (entity == null) return false;
			return Vector3.Distance(transform.position, entity.Transform.position) <= interactableSettings.maxRange + 0.1f;
		}

		public virtual void InteractFrom(IEntity entity, IEnumerator interaction = null)
		{
			if (currentInteractor != null || entity == null) return;

			currentInteractor = entity;


			StartCoroutine(PreInteraction(interaction));
		}

		private IEnumerator PreInteraction(IEnumerator ExternalInteraction = null)
		{
			outline.enabled = false;
			if (!IsInteractorInRange(currentInteractor))
			{
				currentInteractor.SetDestination(GetIteractionPosition(currentInteractor));

				Vector3 lastDestination = currentInteractor.Navigation.CurrentNavMeshDestination;
				bool needBreak = false;

				yield return new WaitWhile(() =>
				{
					if (lastDestination != currentInteractor.Navigation.CurrentNavMeshDestination)
					{
						needBreak = true;
						return false;
					}
					return !currentInteractor.Navigation.NavMeshAgent.IsReachedDestination();
				});

				if (needBreak)
				{
					currentInteractor = null;
					yield break;
				}
			}

			if (IsInteractorInRange(currentInteractor))
			{
				if (ExternalInteraction != null)
				{
					yield return ExternalInteraction;
				}
				else
				{
					yield return InternalInteraction();
				}
			}

			currentInteractor = null;
		}

		protected virtual IEnumerator InternalInteraction()
		{
			yield return null;
		}


		#region Observe
		public virtual void StartObserve()
		{
			outline.enabled = true;
		}
		public virtual void Observe() { }
		public virtual void EndObserve()
		{
			outline.enabled = false;
		}
		#endregion

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			if (interactableSettings.interaction == InteractionType.CustomPoint)
			{
				Gizmos.DrawSphere(transform.TransformPoint(interactableSettings.position), 0.1f);
			}
			else
			{
				Gizmos.DrawWireSphere(transform.position, interactableSettings.maxRange);
			}
		}


		[System.Serializable]
		public class Settings
		{
			public float maxRange = 3f;

			public InteractionType interaction = InteractionType.DirectionPoint;
			[ShowIf("interaction", InteractionType.CustomPoint)]
			public Vector3 position;
		}
	}

	public enum InteractionType
	{
		DirectionPoint,
		CustomPoint,
	}
}