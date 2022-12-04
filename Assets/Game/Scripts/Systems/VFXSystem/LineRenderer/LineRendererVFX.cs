using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

namespace Game.Systems.VFX
{
	public class LineRendererVFX : MonoBehaviour
	{
		public bool IsEnabled { get; protected set; }

		public LineRenderer Line => line;
		[SerializeField] private LineRenderer line;

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

		public virtual void Enable(bool trigger)
		{
			IsEnabled = trigger;
			Line.enabled = IsEnabled;
		}

		public virtual void EnableIn(UnityAction callback = null)
		{
			Enable(true);

			if (Line.material.HasProperty("_Color"))
			{
				Color colorIn = Line.sharedMaterial.color;
				colorIn.a = 1;

				Sequence sequence = DOTween.Sequence();

				sequence
					.Append(Line.sharedMaterial.DOColor(colorIn, 0.25f))
					.OnComplete(() => callback?.Invoke());
			}
			else
			{
				callback?.Invoke();
			}
		}
		public virtual void EnableOut(UnityAction callback = null)
		{
			if (Line.material.HasProperty("_Color"))
			{
				Enable(true);

				Color colorOut = Line.sharedMaterial.color;
				colorOut.a = 0;

				Sequence sequence = DOTween.Sequence();

				sequence
					.Append(Line.sharedMaterial.DOColor(colorOut, 0.2f))
					.OnComplete(() =>
					{
						Enable(false);
						callback?.Invoke();
					});
			}
			else
			{
				Enable(false);
				callback?.Invoke();
			}
		}

		public void Clear()
		{
			line.positionCount = 0;
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