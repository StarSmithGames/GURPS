using UnityEngine;
using DG.Tweening;
using Zenject;
using System.Collections.Generic;

namespace Game.Systems.VFX
{
	public class LineTargetVFX : LineVFX
	{
		[field: SerializeField] public LineRenderer AboveAnimatedLine { get; private set; }
		[Space]
		[SerializeField] private Gradient stateTarget;
		[SerializeField] private Gradient stateTargeted;

		public override void DrawLine(Vector3[] points)
		{
			base.DrawLine(points);

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

		public class Factory : PlaceholderFactory<LineTargetVFX> { }
	}

	public enum LineTargetState
	{
		Target,
		Targeted,
	}
}