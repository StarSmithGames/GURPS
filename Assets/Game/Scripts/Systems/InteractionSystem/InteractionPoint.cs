using Game.Entities.Models;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

		private void OnDrawGizmos()
		{

			Gizmos.color = settings.gizmosEditor;
			if (settings.interaction == InteractionType.CustomPoint)
			{
#if UNITY_EDITOR
				Handles.color = settings.gizmosEditor;
				Handles.DrawSolidDisc(transform.TransformPoint(settings.position), transform.up, 0.25f);
#endif
				//Gizmos.DrawSphere(transform.TransformPoint(settings.position), 0.1f);
			}
			else
			{
				Gizmos.DrawWireSphere(transform.position, settings.maxRange);
			}

#if UNITY_EDITOR
			var style = new GUIStyle();
			style.normal.textColor = settings.gizmosEditor;

			Handles.Label(transform.position + (Vector3.right * settings.maxRange), transform.name, style);
#endif
		}

		[System.Serializable]
		public class Settings
		{
			[Min(0.5f)]
			public float maxRange = 3f;

			public InteractionType interaction = InteractionType.DirectionPoint;
			[ShowIf("interaction", InteractionType.CustomPoint)]
			public Vector3 position;

			[ColorPalette("Gizmos")]
			public Color gizmosEditor;
		}
	}

	public enum InteractionType
	{
		DirectionPoint,
		CustomPoint,
	}
}