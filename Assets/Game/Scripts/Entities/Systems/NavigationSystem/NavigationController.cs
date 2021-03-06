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
		public NavMeshAgent NavMeshAgent { get; private set; }


		public NavigationPath FullPath = new NavigationPath();
		public float FullPathDistance => FullPath.Distance;
		public Vector3 PathDestination => FullPath.EndPoint;


		[HideInInspector] public NavMeshPath CurrentNavMeshPath;
		public float CurrentNavMeshPathDistance => CurrentNavMeshPath.GetPathDistance();
		public Vector3 CurrentNavMeshDestination => NavMeshAgent.pathEndPosition;


		public float NavMeshRemainingDistance => NavMeshAgent.GetPathRemainingDistance();
		public float NavMeshPercentRemainingDistance => NavMeshRemainingDistance / CurrentNavMeshPathDistance;
		public float NavMeshInvertedPercentRemainingDistance => 1 - NavMeshPercentRemainingDistance;

		public bool IsCanReach => (entity.Sheet.Stats.Move.CurrentValue - FullPathDistance) >= 0 && FullPathDistance != 0 && entity.Sheet.Stats.Move.CurrentValue != 0;

		[SerializeField] private Settings settings;

		private SignalBus signalBus;
		private Markers markers;
		private CharacterController3D characterController;
		private IEntity entity;

		[Inject]
        private void Construct(
			SignalBus signalBus,
			NavMeshAgent navMeshAgent,
			Markers markers,
			CharacterController3D characterController,
			IEntity entity)
		{
			this.signalBus = signalBus;
			this.NavMeshAgent = navMeshAgent;
			this.markers = markers;
			this.characterController = characterController;
			this.entity = entity;
		}

		private void Start()
		{
			CurrentNavMeshPath = NavMeshAgent.path;
		}

		private void Update()
		{
			markers.TargetMarker.transform.position = CurrentNavMeshDestination;
			markers.TargetMarker.DrawCircle();

			markers.FollowMarker.DrawCircle();

			markers.LineMarker.DrawLine(NavMeshAgent.path.corners);
		}

		public bool SetTarget(Vector3 destination, float maxPathDistance = -1)
		{
			bool result = false;

			if (NavMeshAgent.IsPathValid(destination))
			{
				NavMeshAgent.CalculatePath(destination, out CurrentNavMeshPath);

				FullPath = new NavigationPath() { Path = CurrentNavMeshPath.corners.ToList() };

				if (maxPathDistance != -1)
				{
					float distance = CurrentNavMeshPathDistance;

					if (distance >= maxPathDistance)
					{
						destination = transform.root.position + (maxPathDistance * (destination - transform.root.position).normalized);//TODO

						if (NavMeshAgent.IsPathValid(destination))
						{
							NavMeshAgent.CalculatePath(destination, out CurrentNavMeshPath);

							result = NavMeshAgent.SetPath(CurrentNavMeshPath);
							return result;
						}
					}
					else if(distance < settings.minPathDistance)
					{
						if((maxPathDistance - settings.minPathDistance) > 0)
						{
							destination = transform.root.position + (settings.minPathDistance * (destination - transform.root.position).normalized);

							if (NavMeshAgent.IsPathValid(destination))
							{
								NavMeshAgent.CalculatePath(destination, out CurrentNavMeshPath);

								result = NavMeshAgent.SetPath(CurrentNavMeshPath);
								return result;
							}
						}
					}
				}

				result = NavMeshAgent.SetPath(CurrentNavMeshPath);
				return result;
			}

			return result;
		}

		private void Validate()
		{
			NavMeshAgent.stoppingDistance = settings.reachTargetThreshold;
		}
		
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, settings.reachTargetThreshold);

			if (!Application.isPlaying) return;

			Gizmos.color = Color.blue;
			for (int i = 0; i < FullPath.Path.Count - 1; i++)
			{
				Gizmos.DrawLine(FullPath.Path[i], FullPath.Path[i + 1]);
			}

			Gizmos.color = Color.red;
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

	public class NavigationPath
	{
		public List<Vector3> Path = new List<Vector3>();

		public Vector3 StartPoint => Path.First();
		public Vector3 EndPoint => Path.Last();
		
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
	}
}