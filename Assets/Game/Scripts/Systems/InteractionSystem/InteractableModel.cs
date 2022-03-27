using Sirenix.OdinInspector;

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems.InteractionSystem
{
	public class InteractableModel : MonoBehaviour, IInteractable
	{
		[SerializeField] protected Settings interactableSettings;

		public Vector3 InteractPosition
		{
			get
			{
				if (interactableSettings.interaction == InteractionType.CustomPoint)
				{
					return transform.TransformPoint(interactableSettings.position);
				}
				else
				{
					if (currentInteractor != null)
					{
						return transform.position + ((interactableSettings.maxRange - 0.1f) * (currentInteractor.Transform.position - transform.position).normalized);
					}
				}

				return transform.position;
			}
		}

		public bool IsInteractable => currentInteractor == null;

		protected IEntity currentInteractor = null;

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