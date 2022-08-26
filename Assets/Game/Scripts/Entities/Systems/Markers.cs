using Game.Systems.VFX;

using System;
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
		[field: SerializeField] public LineRendererCircleVFX FollowMarker { get; private set; }
		[field: SerializeField] public LineRendererCircleVFX TargetMarker { get; private set; }
		[field: SerializeField] public LineRendererCircleVFX AreaMarker { get; private set; }
		[field: SerializeField] public LineRendererSplineVFX SplineMarker { get; private set; }
		[field: SerializeField] public LineRendererSplineVFX AdditionalSplineMarker { get; private set; }
		[field: Space]
		[field: SerializeField] public LineRendererLineVFX LineMarker { get; private set; }
		[field: SerializeField] public LineRendererLineVFX LineErrorMarker { get; private set; }
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

		public void WayLine(NavigationPath path, NavigationPath fullPath, float maxDistance)
		{
			LineMarker.DrawLine(path.Path.ToArray());

			float fullPathDistance = fullPath.Distance;
			var distance = fullPathDistance - maxDistance;

			if (distance > 0)
			{
				var errorLine = fullPath.Copy().Path;
				var endPoint = path.EndPoint;

				var result = errorLine.OrderByDescending((point) => Vector2.Distance(endPoint, point)).First();

				int index = errorLine.IndexOf(result);
				if (index != -1)
				{
					for (int i = 0; i <= index; i++)
					{
						errorLine.RemoveAt(i);
					}
				}

				errorLine[0] = endPoint;


				LineErrorMarker.Enable(true);
				LineErrorMarker.DrawLine(errorLine.ToArray());
			}
			else
			{
				LineErrorMarker.Enable(false);
			}


			//float fullPathDistance = fullPath.Distance;

			//if (maxDistance != 0 && fullPathDistance != 0)
			//{
			//	var distance = fullPathDistance - maxDistance;

			//	if (distance > 0)
			//	{
			//		var redPath = fullPath.Copy();

			//		NavigationPath result = redPath;

			//		while (redPath.Path.Count > 2)
			//		{
			//			Debug.LogError(distance);
			//			if (distance < redPath.Distance)
			//			{
			//				Debug.LogError("RM");
			//				redPath.Path.Remove(redPath.StartPoint);
			//			}
			//			else
			//			{
			//				result = redPath;
			//				break;
			//			}
			//		}

			//		if (result.Path.Count == 2)
			//		{
			//			result[0] = path.EndPoint;
			//		}


			//		LineErrorMarker.Enable(true);
			//		LineErrorMarker.DrawLine(result.Path.ToArray());
			//	}
			//	else
			//	{
			//		LineErrorMarker.Enable(false);
			//	}
			//}
		}

		public Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 direction, Vector2 point)
		{
			direction.Normalize();
			Vector2 lhs = point - origin;

			float dotP = Vector2.Dot(lhs, direction);
			return origin + direction * dotP;
		}

		public Vector2 FindNearestPointOnLine2(Vector2 origin, Vector2 end, Vector2 point)
		{
			//Get heading
			Vector2 heading = (end - origin);
			float magnitudeMax = heading.magnitude;
			heading.Normalize();

			//Do projection from the point but clamp it
			Vector2 lhs = point - origin;
			float dotP = Vector2.Dot(lhs, heading);
			dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
			return origin + heading * dotP;
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