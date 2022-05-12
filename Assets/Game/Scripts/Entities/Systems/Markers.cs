using Game.Systems.VFX;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Entities
{
	public class Markers : MonoBehaviour
	{
		[field: SerializeField] public LineRendererCircleVFX FollowMarker { get; private set; }
		[field: SerializeField] public LineRendererCircleVFX TargetMarker { get; private set; }
		[field: SerializeField] public LineRendererCircleVFX AreaMarker { get; private set; }
		[field: SerializeField] public LineRendererSplineVFX SplineMarker { get; private set; }
		[field: SerializeField] public LineRendererSplineVFX AdditionalSplineMarker { get; private set; }
		[field: Space]
		[field: SerializeField] public LineRendererLineVFX LineMarker { get; private set; }
		[Space]
		[SerializeField] private List<Material> markerColors = new List<Material>();

		[field: Space]
		[field: SerializeField] public IndicatorVFX Exclamation { get; private set; }
		[field: SerializeField] public IndicatorVFX Question { get; private set; }

		private void Awake()
		{
			Assert.AreEqual(markerColors.Count, Enum.GetValues(typeof(MaterialType)).Length, "Materials count are not equal enum");

			TargetMarker.transform.parent = null;
		}

		public void SetFollowMaterial(MaterialType type)
		{
			FollowMarker.Line.material = markerColors[(int)type];
		}
	}

	public enum MaterialType : int
	{
		Leader		= 0,//white
		Companion	= 1,//blue
		Friend		= 2,//green
		Enemy		= 3,//red
	}
}