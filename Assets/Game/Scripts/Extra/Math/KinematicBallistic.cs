using UnityEngine;

public static class KinematicBallistic
{
	private static float g = Physics.gravity.y;//усорение свободного падения

	public static Vector3[] GetTraectory(Vector3 from, Vector3 to, int resolution = 100)
	{
		return GetTraectory(from, to, Mathf.Max(0.5f, to.y + 1.5f), resolution);
	}

	public static Vector3[] GetTraectory(Vector3 from, Vector3 to, float height, int resolution = 100)
	{
		CalculateVelocity(to - from, height, out Vector3 velocity, out float time);

		Vector3[] points = new Vector3[resolution];
		points[0] = from;
		for (int i = 1; i < points.Length; i++)
		{
			float simulationTime = i / (float)points.Length * time;
			Vector3 displacement = velocity * simulationTime + Vector3.up * g * simulationTime * simulationTime / 2f;
			points[i] = from + displacement;
		}

		return points;
	}

	private static void CalculateVelocity(Vector3 direction, float height, out Vector3 velocity, out float time)
	{
		time = Mathf.Sqrt(-2 * height / g) + Mathf.Sqrt(2 * (direction.y - height) / g);
		velocity = new Vector3(direction.x / time, Mathf.Sqrt(-2 * height * g) * -Mathf.Sign(g), direction.z / time);
	}
}