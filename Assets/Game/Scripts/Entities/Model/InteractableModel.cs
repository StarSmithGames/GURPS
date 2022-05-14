using EPOOutline;

using Game.Entities;

using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

namespace Game.Systems.InteractionSystem
{
	public class InteractableModel : MonoBehaviour, IInteractable, IObservable
	{
		[SerializeField] protected Settings interactableSettings;

		public bool IsInteractable => outline.enabled;

		protected IEntity interactorInitiator = null;//тот кто взаимодействует с этим объектом

		private Outlinable outline;

		[Inject]
		private void Construct(Outlinable outline)
		{
			this.outline = outline;
		}

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
					if (IsInRange(entity)) return entity.Transform.position;

					return transform.position + ((interactableSettings.maxRange - 0.1f) * (entity.Transform.position - transform.position).normalized);
				}
			}

			return transform.position;
		}

		public bool IsInRange(IEntity entity)
		{
			if (entity == null) return false;

			float distance = Vector3.Distance(transform.position, entity.Transform.position);

			return distance <= interactableSettings.maxRange + 0.1f;
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