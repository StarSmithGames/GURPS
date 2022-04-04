using EPOOutline;

using Game.Entities;

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
			return Vector3.Distance(transform.position, entity.Transform.position) <= interactableSettings.maxRange;
		}

		protected IEntity currentInteractor = null;
		public void InteractFrom(IEntity entity)
		{
			if (currentInteractor != null || entity == null)
			{
				return;
			}

			currentInteractor = entity;

			StartCoroutine(PreInteraction());
		}
		private IEnumerator PreInteraction()
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

			yield return Interaction();

			currentInteractor = null;
		}

		protected virtual IEnumerator Interaction()
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