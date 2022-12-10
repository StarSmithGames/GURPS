using UnityEngine;
using DG.Tweening;
using Zenject;
using System.Collections.Generic;

namespace Game.Systems.VFX
{
	public class LineTargetVFX : PoolableObject
	{
		[field: SerializeField] public LineRenderer BaseLine { get; private set; }
		[field: SerializeField] public LineRenderer AboveAnimatedLine { get; private set; }
		[Space]
		[SerializeField] private Gradient stateTarget;
		[SerializeField] private Gradient stateTargeted;

		public void DrawLine(Vector3[] points)
		{
			if (points == null) return;

			DrawLine(BaseLine, points);
			DrawLine(AboveAnimatedLine, points);
		}

		public LineTargetVFX SetState(LineTargetState state)
		{
			if(state == LineTargetState.Target)
			{
				AboveAnimatedLine.enabled = true;

				BaseLine.colorGradient = stateTarget;
			}
			else if(state == LineTargetState.Targeted)
			{
				AboveAnimatedLine.enabled = false;
				BaseLine.colorGradient = stateTargeted;
			}

			return this;
		}

		public void FadeOut()
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
			0f, 0.25f)
			.OnComplete(DespawnIt);
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

		private void DrawLine(LineRenderer line, Vector3[] points)
		{
			line.loop = false;
			line.useWorldSpace = true;

			line.positionCount = points.Length;

			line.SetPositions(points);
		}

		public class Factory : PlaceholderFactory<LineTargetVFX> { }
	}

	public enum LineTargetState
	{
		Target,
		Targeted,
	}
}