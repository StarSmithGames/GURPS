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

		public float CurrentPathDistance => CurrentPath.Distance;
		public float CurrentNavMeshPathDistance => CurrentNavMeshPath.GetPathDistance();
		public Vector3 CurrentNavMeshDestination => NavMeshAgent.pathEndPosition;

		public float NavMeshRemainingDistance => NavMeshAgent.GetPathRemainingDistance();
		public float NavMeshPercentRemainingDistance => NavMeshRemainingDistance / CurrentNavMeshPathDistance;
		public float NavMeshInvertedPercentRemainingDistance => 1 - NavMeshPercentRemainingDistance;

		[SerializeField] private Settings settings;

		private NavMeshPath CurrentNavMeshPath;
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
			CurrentNavMeshPath = NavMeshAgent.path;
		}

		public bool SetTarget(Vector3 destination, float maxPathDistance = -1)
		{
			bool result = false;

			if (NavMeshAgent.IsPathValid(destination))
			{
				NavMeshAgent.CalculatePath(destination, out CurrentNavMeshPath);

				FullPath = new NavigationPath(CurrentNavMeshPath.corners);

				if (maxPathDistance != -1)
				{
					float distance = CurrentNavMeshPathDistance;

					if (distance > maxPathDistance)
					{
						//destination = root.position + (maxPathDistance * (destination - root.position).normalized);//TODO
						destination = root.position + ((maxPathDistance - 0.5f) * (destination - root.position).normalized);//Need lil Fix

						if (NavMeshAgent.IsPathValid(destination))
						{
							NavMeshAgent.CalculatePath(destination, out NavMeshPath path);

							if (path.GetPathDistance() > maxPathDistance)//ignore path over distance
							{
								return false;
							}
						}
					}
					else if(distance < settings.minPathDistance)
					{
						if((maxPathDistance - settings.minPathDistance) > 0)
						{
							destination = root.position + (settings.minPathDistance * (destination - root.position).normalized);
						}
					}
				}

				if (NavMeshAgent.IsPathValid(destination))
				{
					NavMeshAgent.CalculatePath(destination, out CurrentNavMeshPath);
					CurrentPath = new NavigationPath(CurrentNavMeshPath.corners);
					result = NavMeshAgent.SetPath(CurrentNavMeshPath);
					return result;
				}
			}

			return result;
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

			Gizmos.color = Color.blue;
			for (int i = 0; i < CurrentNavMeshPath.corners.Length - 1; i++)
			{
				Gizmos.DrawLine(CurrentNavMeshPath.corners[i], CurrentNavMeshPath.corners[i + 1]);
			}
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