using Game.Systems.VFX;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game.Entities
{
	public class Markers : MonoBehaviour
	{
		[field: SerializeField] public DecalVFX FollowDecal { get; private set; }
		[field: SerializeField] public DecalVFX TargetDecal { get; private set; }
		[field: SerializeField] public DecalVFX AreaDecal { get; private set; }
		[field: SerializeField] public LineRendererSplineVFX SplineMarker { get; private set; }
		[field: SerializeField] public LineRendererSplineVFX AdditionalSplineMarker { get; private set; }
		[field: Space]
		[field: SerializeField] public LineRendererLineVFX LineMarker { get; private set; }
		[Space]
		[SerializeField] private List<Material> markerColors = new List<Material>();//rm
		[SerializeField] private List<Color> colors = new List<Color>();

		[field: Space]
		[field: SerializeField] public IndicatorVFX Exclamation { get; private set; }
		[field: SerializeField] public IndicatorVFX Question { get; private set; }

		private void Awake()
		{
			Assert.AreEqual(markerColors.Count, Enum.GetValues(typeof(MaterialType)).Length, "Materials count are not equal enum");

			TargetDecal.transform.parent = null;
		}

		public void SetFollowMaterial(MaterialType type)
		{
			FollowDecal.SetColor(colors[(int)type]);
		}
	}

	public static class MarkersUsage
	{
		public static void Reset(this Markers markers)
		{
			markers.FollowDecal.Enable(false);

			markers.TargetDecal.transform.parent = null;
			markers.TargetDecal.Enable(false);

			markers.AreaDecal.Enable(false);

			markers.SplineMarker.Enable(false);
			markers.AdditionalSplineMarker.Enable(false);

			markers.LineMarker.Enable(false);

			markers.Exclamation.Enable(false);
			markers.Question.Enable(false);
		}

		public static void EnableSingleTarget(this Markers markers, bool trigger)
		{
			markers.LineMarker.Enable(trigger);
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