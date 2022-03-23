using Game.Systems.VFX;

using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;

namespace Game.Systems.VFX
{
	public class LineRendererLineVFX : LineRendererVFX
	{
		public bool useSmooth = false;

		public void DrawLine(Vector3[] points)
		{
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

		void DrawSineWave(Vector3 startPoint, float amplitude, float wavelength)
		{
			float x = 0f;
			float y;
			float k = 2 * Mathf.PI / wavelength;
			Line.positionCount = 200;
			for (int i = 0; i < Line.positionCount; i++)
			{
				x += i * 0.001f;
				y = amplitude * Mathf.Sin(k * x);
				Line.SetPosition(i, new Vector3(x, y, 0) + startPoint);
			}
		}

		void DrawTravellingSineWave(Vector3 startPoint, float amplitude, float wavelength, float waveSpeed)
		{

			float x = 0f;
			float y;
			float k = 2 * Mathf.PI / wavelength;
			float w = k * waveSpeed;
			Line.positionCount = 200;
			for (int i = 0; i < Line.positionCount; i++)
			{
				x += i * 0.001f;
				y = amplitude * Mathf.Sin(k * x + w * Time.time);
				Line.SetPosition(i, new Vector3(x, y, 0) + startPoint);
			}
		}

		void DrawStandingSineWave(Vector3 startPoint, float amplitude, float wavelength, float waveSpeed)
		{

			float x = 0f;
			float y;
			float k = 2 * Mathf.PI / wavelength;
			float w = k * waveSpeed;
			Line.positionCount = 200;
			for (int i = 0; i < Line.positionCount; i++)
			{
				x += i * 0.001f;
				y = amplitude * (Mathf.Sin(k * x + w * Time.time) + Mathf.Sin(k * x - w * Time.time));
				Line.SetPosition(i, new Vector3(x, y, 0) + startPoint);
			}
		}

		void DrawQuadraticBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2)
		{
			Line.positionCount = 200;
			float t = 0f;
			Vector3 B = new Vector3(0, 0, 0);
			for (int i = 0; i < Line.positionCount; i++)
			{
				B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
				Line.SetPosition(i, B);
				t += (1 / (float)Line.positionCount);
			}
		}

		void DrawCubicBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3)
		{

			Line.positionCount = 200;
			float t = 0f;
			Vector3 B = new Vector3(0, 0, 0);
			for (int i = 0; i < Line.positionCount; i++)
			{
				B = (1 - t) * (1 - t) * (1 - t) * point0 + 3 * (1 - t) * (1 - t) *
					t * point1 + 3 * (1 - t) * t * t * point2 + t * t * t * point3;

				Line.SetPosition(i, B);
				t += (1 / (float)Line.positionCount);
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