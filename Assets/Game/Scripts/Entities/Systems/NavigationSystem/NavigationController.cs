using Game.Managers.GameManager;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;

using Zenject;

namespace Game.Entities
{
	public class NavigationController : MonoBehaviour
    {
		public NavigationPath FullPath = new NavigationPath();
		public NavigationPath CurrentPath = new NavigationPath();

		public NavMeshAgent NavMeshAgent { get; private set; }

		public float CurrentNavMeshPathDistance => CurrentPath.Distance; //CurrentNavMeshPath.GetPathDistance();
		public Vector3 CurrentNavMeshDestination => CurrentPath.EndPoint;

		public float NavMeshRemainingDistance => NavMeshAgent.GetPathRemainingDistance();
		public float NavMeshPercentRemainingDistance => NavMeshRemainingDistance / CurrentNavMeshPathDistance;
		public float NavMeshInvertedPercentRemainingDistance => 1 - NavMeshPercentRemainingDistance;

		[SerializeField] private Settings settings;

		private Transform root;

		[Inject]
        private void Construct(NavMeshAgent navMeshAgent)
		{
			NavMeshAgent = navMeshAgent;
		}

		private void Start()
		{
			root = transform.root;

			NavMeshAgent.stoppingDistance = settings.reachTargetThreshold;
			CurrentPath = new NavigationPath(NavMeshAgent.path.corners);
		}

		public bool SetTarget(Vector3 destination, float maxPathDistance = -1)
		{
			bool result = false;

			if (NavMeshAgent.IsPathValid(destination))
			{
				NavMeshAgent.CalculatePath(destination, out NavMeshPath fullPath);
				FullPath = new NavigationPath(fullPath.corners);

				if (maxPathDistance != -1)
				{
					NavMeshAgent.CalculatePath(destination, out NavMeshPath navMeshPath);
					var path = new NavigationPath(navMeshPath.corners);
					float distance = path.Distance;

					if (distance >= maxPathDistance)
					{
						destination = root.position + (maxPathDistance * (destination - root.position).normalized);//TODO need destination on distance
						//destination = root.position + ((maxPathDistance - 0.1f) * (destination - root.position).normalized);//Need lil Fix

						//if (NavMeshAgent.IsPathValid(destination))
						//{
						//	NavMeshAgent.CalculatePath(destination, out NavMeshPath path);

						//	if (path.GetPathDistance() > maxPathDistance)//ignore path over distance
						//	{
						//		return false;
						//	}
						//}
					}
					else if(distance < settings.minPathDistance)
					{
						if((maxPathDistance - settings.minPathDistance) > 0)
						{
							destination = root.position + ((settings.minPathDistance - 0.1f) * (destination - root.position).normalized);
						}
					}
				}

				if (NavMeshAgent.IsPathValid(destination))
				{
					NavMeshAgent.CalculatePath(destination, out NavMeshPath currentPath);
					CurrentPath = new NavigationPath(currentPath.corners);

					result = NavMeshAgent.SetPath(currentPath);
					return result;
				}
			}

			return result;
		}

		public Vector3 FindPointAlongPath(NavigationPath navigationPath, float distanceToTravel)
		{
			Vector3[] path = navigationPath.Path.ToArray();

			if (distanceToTravel < 0)
			{
				return path[0];
			}

			//Loop Through Each Corner in Path
			for (int i = 0; i < path.Length - 1; i++)
			{
				//If the distance between the next to points is less than the distance you have left to travel
				if (distanceToTravel <= Vector3.Distance(path[i], path[i + 1]) / navigationPath.Distance)
				{
					//Calculate the point that is the correct distance between the two points and return it
					Vector3 directionToTravel = path[i + 1] - path[i];
					directionToTravel.Normalize();
					return (path[i] + (directionToTravel * distanceToTravel));
				}
				else
				{
					//otherwise subtract the distance between those 2 points from the distance left to travel
					distanceToTravel -= Vector3.Distance(path[i], path[i + 1]) / navigationPath.Distance;
				}
			}

			//if the distance to travel is greater than the distance of the path, return the final point
			return path[path.Length - 1];
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


		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, settings.reachTargetThreshold);

			if (!Application.isPlaying) return;

			Gizmos.color = Color.red;
			for (int i = 0; i < FullPath.Path.Count - 1; i++)
			{
				Gizmos.DrawLine(FullPath.Path[i], FullPath.Path[i + 1]);
			}

			//Gizmos.color = Color.blue;
			//for (int i = 0; i < CurrentNavMeshPath.corners.Length - 1; i++)
			//{
			//	Gizmos.DrawLine(CurrentNavMeshPath.corners[i], CurrentNavMeshPath.corners[i + 1]);
			//}
		}

		[System.Serializable]
		public class Settings
		{
			public float reachTargetThreshold = 0.1f;
			public float minPathDistance = 1f;
		}
	}

	public class NavigationPath : ICopyable<NavigationPath>
	{
		public List<Vector3> Path = new List<Vector3>();

		public Vector3 StartPoint => Path.First();
		public Vector3 EndPoint => Path.Last();

		public Vector3 this[int i] {
			
			get => Path[i];
			set => Path[i] = value;
		}
		
		public float Distance
		{
			get
			{
				float distance = 0.0f;
				for (int i = 0; i < Path.Count - 1; ++i)
				{
					distance += Vector3.Distance(Path[i], Path[i + 1]);
				}

				return distance;
			}
		}

		public NavigationPath()
		{
			Path = new List<Vector3>();
		}

		public NavigationPath(Vector3[] path)
		{
			Path = new List<Vector3>(path);
		}
		public NavigationPath(List<Vector3> path)
		{
			Path = new List<Vector3>(path);
		}


		public NavigationPath Copy()
		{
			return new NavigationPath(Path);
		}
	}
}