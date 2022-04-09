using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;

namespace Game.Systems.VFX
{
	public class LineRendererVFX : MonoBehaviour
	{
		public bool IsEnabled { get; protected set; }

		[field: SerializeField] public LineRenderer Line { get; set; }

		[SerializeField] protected Vector3 lookFrom = new Vector3(0, 1f, 0);
		[SerializeField] protected Vector3 offset = new Vector3(0, 0.01f, 0);

		[SerializeField] protected bool isDebub = false;
		[ShowIf("isDebub")]
		[SerializeField] protected bool showLines = false;

		[SerializeField] protected bool useRaycast = true;
		[ShowIf("useRaycast")]
		[SerializeField] protected float rayLength = 100f;
		[ShowIf("useRaycast")]
		[SerializeField] protected LayerMask raycastLayerMask = ~0;

		public void Enable(bool trigger)
		{
			IsEnabled = trigger;
			Line.enabled = IsEnabled;
		}

		public void EnableIn()
		{
			Sequence sequence = DOTween.Sequence();

			Color color = Line.material.color;
			color.a = 0;
			Line.material.color = color;

			Enable(true);

			sequence
				.Append(Line.material.DOFade(1, 0.25f));
		}
		public void EnableOut()
		{
			IsEnabled = false;

			Sequence sequence = DOTween.Sequence();

			sequence
				.Append(Line.material.DOFade(0, 0.25f))
				.AppendCallback(() => Enable(false));
		}

		void DrawTriangle(Vector3[] vertexPositions, float startWidth, float endWidth)
		{
			Line.startWidth = startWidth;
			Line.endWidth = endWidth;
			Line.loop = true;
			Line.positionCount = 3;
			Line.SetPositions(vertexPositions);
		}

		void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth)
		{
			Line.startWidth = startWidth;
			Line.endWidth = endWidth;
			Line.loop = true;
			float angle = 2 * Mathf.PI / vertexNumber;
			Line.positionCount = vertexNumber;

			for (int i = 0; i < vertexNumber; i++)
			{
				Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
														 new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
										   new Vector4(0, 0, 1, 0),
										   new Vector4(0, 0, 0, 1));
				Vector3 initialRelativePosition = new Vector3(0, radius, 0);
				Line.SetPosition(i, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));
			}
		}

		protected virtual void OnDrawGizmosSelected()
		{
			if (isDebub)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(transform.position + lookFrom, transform.position + lookFrom + transform.forward);

				Gizmos.color = Color.red;
				Gizmos.DrawSphere(transform.position + lookFrom, 0.05f);
				Gizmos.DrawLine(transform.position, transform.position + lookFrom);

				if (showLines)
				{
					Gizmos.color = Color.white;
					for (int i = 0; i < Line.positionCount; i++)
					{
						Gizmos.DrawLine(transform.TransformPoint(Line.GetPosition(i)) + new Vector3(0, lookFrom.y, 0), transform.TransformPoint(Line.GetPosition(i)) + lookFrom + Vector3.down * rayLength);
					}
				}
			}
		}
	}
}