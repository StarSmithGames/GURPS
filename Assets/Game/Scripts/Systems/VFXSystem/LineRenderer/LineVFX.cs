using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Systems.VFX
{
	public abstract class LineVFX : PoolableObject
	{
		public bool IsEnabled { get; private set; }

		[field: SerializeField] public LineRenderer BaseLine { get; private set; }

		private bool isHasSpeed;
		private float speed;

		private void Start()
		{
			isHasSpeed = BaseLine.sharedMaterial.HasFloat("_Speed");
			if (isHasSpeed)
			{
				speed = BaseLine.sharedMaterial.GetFloat("_Speed");
			}

			IsEnabled = true;
		}

		public virtual void FadeTo(float endValue, bool despawnItOnEnd = false, UnityAction callback = null)
		{
			Color startColor = BaseLine.startColor;
			Color endColor = BaseLine.endColor;

			DOTween.To(() => startColor.a,
			x => {
				startColor.a = x;
				endColor.a = x;

				BaseLine.startColor = startColor;
				BaseLine.endColor = endColor;
			},
			endValue, 0.25f)
			.OnComplete(() =>
			{
				IsEnabled = endValue != 0;

				if (despawnItOnEnd)
				{
					DespawnIt();
				}
				callback?.Invoke();
			});
		}


		public virtual void DrawLine(Vector3[] points)
		{
			DrawLine(BaseLine, points);
		}

		protected virtual void DrawLine(LineRenderer line, Vector3[] points)
		{
			if (points == null) return;

			line.loop = false;
			line.useWorldSpace = true;

			line.positionCount = points.Length;

			line.SetPositions(points);
		}

		public void SetMaterialSpeed(float speed)
		{
			if (!isHasSpeed) return;
		
			BaseLine.sharedMaterial.SetFloat("_Speed", speed);
		}

		public void SetMaterialSpeedToDefault()
		{
			SetMaterialSpeed(speed);
		}

		public override void OnSpawned(IMemoryPool pool)
		{
			Color startColor = BaseLine.startColor;
			Color endColor = BaseLine.endColor;

			startColor.a = 1f;
			endColor.a = 1f;

			BaseLine.startColor = startColor;
			BaseLine.endColor = endColor;

			base.OnSpawned(pool);
		}
	}
}