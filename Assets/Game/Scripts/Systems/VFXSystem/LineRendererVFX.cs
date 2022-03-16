using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class LineRendererVFX : VFX
{
	public LineRenderer line;
	[OnValueChanged("DrawCircle")]
	[Min(0)]
	public int steps;
	[OnValueChanged("DrawCircle")]
	[Min(0)]
	public float radius;

	public Vector3 lookFrom = new Vector3(0, 1f, 0);
	public Vector3 offset = new Vector3(0, 0.01f, 0);

	public bool isDebub = false;
	public bool useRaycast = true;
	public float rayLength = 100f;
	public LayerMask raycastLayerMask = ~0;


	[Button("Update")]
	private void DrawCircle()
	{
		line.loop = true;
		line.positionCount = steps;

		float TAU = 2 * Mathf.PI;

		for (int i = 0; i < steps; i++)
		{
			float radians = ((float)i / steps) * TAU;
			Vector3 position = new Vector3(Mathf.Cos(radians) * radius, 0, Mathf.Sin(radians) * radius);

			if (useRaycast)
			{
				Ray ray = new Ray(transform.TransformPoint(position) + lookFrom, Vector3.down);
				if (Physics.Raycast(ray, out RaycastHit hit, rayLength, raycastLayerMask, QueryTriggerInteraction.Ignore))
				{
					position.y = hit.point.y;
				}
			}

			line.SetPosition(i, position + offset);
		}
	}

	void DrawTriangle(Vector3[] vertexPositions, float startWidth, float endWidth)
	{
		line.startWidth = startWidth;
		line.endWidth = endWidth;
		line.loop = true;
		line.positionCount = 3;
		line.SetPositions(vertexPositions);
	}

	void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth)
	{
		line.startWidth = startWidth;
		line.endWidth = endWidth;
		line.loop = true;
		float angle = 2 * Mathf.PI / vertexNumber;
		line.positionCount = vertexNumber;

		for (int i = 0; i < vertexNumber; i++)
		{
			Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
													 new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
									   new Vector4(0, 0, 1, 0),
									   new Vector4(0, 0, 0, 1));
			Vector3 initialRelativePosition = new Vector3(0, radius, 0);
			line.SetPosition(i, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));
		}
	}

	void DrawSineWave(Vector3 startPoint, float amplitude, float wavelength)
	{
		float x = 0f;
		float y;
		float k = 2 * Mathf.PI / wavelength;
		line.positionCount = 200;
		for (int i = 0; i < line.positionCount; i++)
		{
			x += i * 0.001f;
			y = amplitude * Mathf.Sin(k * x);
			line.SetPosition(i, new Vector3(x, y, 0) + startPoint);
		}
	}

	void DrawTravellingSineWave(Vector3 startPoint, float amplitude, float wavelength, float waveSpeed)
	{

		float x = 0f;
		float y;
		float k = 2 * Mathf.PI / wavelength;
		float w = k * waveSpeed;
		line.positionCount = 200;
		for (int i = 0; i < line.positionCount; i++)
		{
			x += i * 0.001f;
			y = amplitude * Mathf.Sin(k * x + w * Time.time);
			line.SetPosition(i, new Vector3(x, y, 0) + startPoint);
		}
	}

	void DrawStandingSineWave(Vector3 startPoint, float amplitude, float wavelength, float waveSpeed)
	{

		float x = 0f;
		float y;
		float k = 2 * Mathf.PI / wavelength;
		float w = k * waveSpeed;
		line.positionCount = 200;
		for (int i = 0; i < line.positionCount; i++)
		{
			x += i * 0.001f;
			y = amplitude * (Mathf.Sin(k * x + w * Time.time) + Mathf.Sin(k * x - w * Time.time));
			line.SetPosition(i, new Vector3(x, y, 0) + startPoint);
		}
	}

	void DrawQuadraticBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2)
	{
		line.positionCount = 200;
		float t = 0f;
		Vector3 B = new Vector3(0, 0, 0);
		for (int i = 0; i < line.positionCount; i++)
		{
			B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
			line.SetPosition(i, B);
			t += (1 / (float)line.positionCount);
		}
	}

	void DrawCubicBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3)
	{

		line.positionCount = 200;
		float t = 0f;
		Vector3 B = new Vector3(0, 0, 0);
		for (int i = 0; i < line.positionCount; i++)
		{
			B = (1 - t) * (1 - t) * (1 - t) * point0 + 3 * (1 - t) * (1 - t) *
				t * point1 + 3 * (1 - t) * t * t * point2 + t * t * t * point3;

			line.SetPosition(i, B);
			t += (1 / (float)line.positionCount);
		}
	}


	private void OnDrawGizmos()
	{
		DrawCircle();

		if (isDebub)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position + lookFrom, transform.position + lookFrom + transform.forward);

			Gizmos.color = Color.red;
			Gizmos.DrawSphere(transform.position + lookFrom, 0.1f);
			Gizmos.DrawLine(transform.position, transform.position + lookFrom);

			Gizmos.color = Color.white;
			for (int i = 0; i < line.positionCount; i++)
			{
				Gizmos.DrawLine(transform.TransformPoint(line.GetPosition(i)) + new Vector3(0, lookFrom.y, 0), transform.TransformPoint(line.GetPosition(i)) + lookFrom + Vector3.down * rayLength);
			}
		}
	}
}