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
		[field: Space]
		[field: SerializeField] public LineRendererLineVFX LineMarker { get; private set; }
		[Space]
		[SerializeField] private List<Material> materials = new List<Material>();

		private void Awake()
		{
			Assert.AreEqual(materials.Count, Enum.GetValues(typeof(MaterialType)).Length, "Materials count are not equal enum");
		}

		public void SetFollowMaterial(MaterialType type)
		{
			FollowMarker.Line.material = materials[(int)type];
		}
	}

	public enum MaterialType : int
	{
		Leader		= 0,
		Companion	= 1,
		Friend		= 2,
		Enemy		= 3,
	}
}