using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Systems.VFX
{
	public class LineRendererCircleVFX : LineRendererVFX
	{
		[OnValueChanged("DrawCircle")]
		[Min(0)]
		[SerializeField] private int pointCount;
		[OnValueChanged("DrawCircle")]
		[Min(0)]
		[SerializeField] private float radius;

		public void DrawCircle()
		{
			if (!IsEnabled) return;

			Line.loop = true;
			Line.positionCount = pointCount;

			float TAU = 2 * Mathf.PI;

			for (int i = 0; i < pointCount; i++)
			{
				float radians = ((float)i / pointCount) * TAU;
				Vector3 position = new Vector3(Mathf.Cos(radians) * radius, 0, Mathf.Sin(radians) * radius);

				if (useRaycast)
				{
					Ray ray = new Ray(transform.root.TransformPoint(position) + lookFrom, Vector3.down);
					if (Physics.Raycast(ray, out RaycastHit hit, rayLength, raycastLayerMask, QueryTriggerInteraction.Ignore))
					{
						position.y = hit.point.y - transform.root.position.y;
					}
					else
					{
						position.y = 0;
					}
				}

				Line.SetPosition(i, position + offset);
			}
		}

		protected override void OnDrawGizmos()
		{
			if (Application.isEditor)
			{
				IsEnabled = true;
				DrawCircle();
			}

			base.OnDrawGizmos();
		}
	}
}