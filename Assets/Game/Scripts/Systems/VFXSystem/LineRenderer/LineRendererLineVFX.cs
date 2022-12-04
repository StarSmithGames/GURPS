using UnityEngine;

namespace Game.Systems.VFX
{
	public class LineRendererLineVFX : LineRendererVFX
	{
		public bool useSmooth = false;

		private float speed;

		private void Start()
		{
			speed = Line.material.GetFloat("_Speed");
		}

		public void SetMaterialSpeed(float speed)
		{
			Line.material.SetFloat("_Speed", speed);
		}

		public void SetMaterialSpeedToDefault()
		{
			SetMaterialSpeed(speed);
		}


		public void DrawLine(Vector3[] points)
		{
			if (!IsEnabled) return;
			if (points == null) return;

			Line.loop = false;
			Line.useWorldSpace = true;

			Line.positionCount = points.Length;

			if (useRaycast)
			{
				for (int i = 0; i < points.Length; i++)
				{
					Ray ray = new Ray(transform.TransformPoint(points[i]) + new Vector3(0, lookFrom.y, 0), Vector3.down);
					if (Physics.Raycast(ray, out RaycastHit hit, rayLength, raycastLayerMask, QueryTriggerInteraction.Ignore))
					{
						points[i].y = hit.point.y;
					}
					else
					{
						points[i].y = 0;
					}

					Line.SetPosition(i, points[i] + offset);
				}
			}
			else
			{
				Line.SetPositions(points);
			}
		}
	}
}

[System.Serializable]
public class BezierCurve
{
	public Vector3[] Points;

	public BezierCurve()
	{
		Points = new Vector3[4];
	}

	public BezierCurve(Vector3[] Points)
	{
		this.Points = Points;
	}

	public Vector3 StartPosition
	{
		get
		{
			return Points[0];
		}
	}

	public Vector3 EndPosition
	{
		get
		{
			return Points[3];
		}
	}

	// Equations from: https://en.wikipedia.org/wiki/B%C3%A9zier_curve
	public Vector3 GetSegment(float Time)
	{
		Time = Mathf.Clamp01(Time);
		float time = 1 - Time;
		return (time * time * time * Points[0])
			+ (3 * time * time * Time * Points[1])
			+ (3 * time * Time * Time * Points[2])
			+ (Time * Time * Time * Points[3]);
	}

	public Vector3[] GetSegments(int Subdivisions)
	{
		Vector3[] segments = new Vector3[Subdivisions];

		float time;
		for (int i = 0; i < Subdivisions; i++)
		{
			time = (float)i / Subdivisions;
			segments[i] = GetSegment(time);
		}

		return segments;
	}
}