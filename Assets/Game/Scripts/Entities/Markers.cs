using Game.Systems.VFX;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Entities
{
	public partial class Markers : MonoBehaviour
	{
		[SerializeField] private List<Color> colors = new List<Color>();

		[field: Space]
		[field: SerializeField] public IndicatorVFX Exclamation { get; private set; }
		[field: SerializeField] public IndicatorVFX Question { get; private set; }

		private void Start()
		{
			Assert.AreEqual(colors.Count, Enum.GetValues(typeof(MaterialType)).Length, "Colors count are not equal enum");
		}

		public void SetFollowMaterial(MaterialType type)
		{
			//FollowDecal.SetColor(colors[(int)type]);
		}
	}

	public static class MarkersUsage
	{
		public static void Reset(this Markers markers)
		{
			markers.Exclamation.Enable(false);
			markers.Question.Enable(false);
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