using EPOOutline;

using Game.Entities;

using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.InteractionSystem
{
	public class InteractableModel : MonoBehaviour, IInteractable, IObservable
	{
		[SerializeField] protected Settings interactableSettings;
		[Space]
		[SerializeField] protected Outlinable outline;

		public bool IsInteractable => currentInteractor == null;

		protected IEntity currentInteractor = null;

		private void Awake()
		{
			outline.enabled = false;
		}

		public Vector3 GetIteractionPosition(IEntity entity = null)
		{
			currentInteractor = entity;

			if (interactableSettings.interaction == InteractionType.CustomPoint)
			{
				return transform.TransformPoint(interactableSettings.position);
			}
			else
			{
				if (entity != null)
				{
					if (IsInteractorInRange()) return entity.Transform.position;

					return transform.position + ((interactableSettings.maxRange - 0.1f) * (entity.Transform.position - transform.position).normalized);
				}
			}

			return transform.position;
		}

		public void Interact() { }

		public virtual void InteractFrom(IEntity entity)
		{
			if (currentInteractor != null || entity == null)
			{
				return;
			}

			currentInteractor = entity;
		}

		protected bool IsInteractorInRange()
		{
			if (currentInteractor == null) return false;
			return Vector3.Distance(transform.position, currentInteractor.Transform.position) <= interactableSettings.maxRange;
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