using Game.Entities;
using Game.Entities.Models;

using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.InteractionSystem
{
	public class InteractionPoint : MonoBehaviour
	{
		[SerializeField] private Settings settings;

		public Vector3 GetIteractionPosition(IEntityModel entity = null)
		{
			if (settings.interaction == InteractionType.CustomPoint)
			{
				return transform.TransformPoint(settings.position);
			}
			else
			{
				if (entity != null)
				{
					if (IsInRange(entity.Transform.position)) return entity.Transform.position;

					return transform.position + ((settings.maxRange - 0.1f) * (entity.Transform.position - transform.position).normalized);
				}
			}

			return transform.position;
		}

		public bool IsInRange(Vector3 position)
		{
			float distance = Vector3.Distance(transform.position, position);

			return distance <= settings.maxRange + 0.1f;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			if (settings.interaction == InteractionType.CustomPoint)
			{
				Gizmos.DrawSphere(transform.TransformPoint(settings.position), 0.1f);
			}
			else
			{
				Gizmos.DrawWireSphere(transform.position, settings.maxRange);
			}
		}

		[System.Serializable]
		public class Settings
		{
			[Min(0.5f)]
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