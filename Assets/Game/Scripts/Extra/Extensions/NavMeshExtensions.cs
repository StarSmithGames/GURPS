using UnityEngine;
using UnityEngine.AI;

public static class NavMeshExtensions
{
	public static bool IsReachedDestination(this NavMeshAgent navMeshAgent)
	{
		return (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) && !navMeshAgent.pathPending;
	}
	public static bool IsReachesDestination(this NavMeshAgent navMeshAgent)
	{
		return (navMeshAgent.remainingDistance >= navMeshAgent.stoppingDistance) && !navMeshAgent.pathPending;
	}

	public static bool CalculatePath(this NavMeshAgent navMeshAgent, Vector3 destination, out NavMeshPath path)
	{
		path = new NavMeshPath();
		navMeshAgent.CalculatePath(destination, path);
		return path.status == NavMeshPathStatus.PathComplete;
	}
	public static bool IsPathValid(this NavMeshAgent navMeshAgent, Vector3 destination)
	{
		return CalculatePath(navMeshAgent, destination, out NavMeshPath path);
	}

	public static float GetPathDistance(this NavMeshPath path)
	{
		if (path == null || path.status == NavMeshPathStatus.PathInvalid || path.corners.Length == 0) return -1f;

		float distance = 0.0f;
		for (int i = 0; i < path.corners.Length - 1; ++i)
		{
			distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
		}

		return distance;
	}
	public static float GetPathRemainingDistance(this NavMeshAgent navMeshAgent)
	{
		if (navMeshAgent.pathPending) return -1f;
		return GetPathDistance(navMeshAgent.path);
	}
}
