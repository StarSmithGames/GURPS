using Game.Managers.GameManager;

using UnityEngine;
using UnityEngine.AI;

using Zenject;

namespace Game.Entities
{
    public class NavigationController : MonoBehaviour
    {
		public Vector3 CurrentNavMeshDestination => navMeshAgent.pathEndPosition;

		public Vector3[] CurrentNavMeshPath => navMeshAgent.path.corners;

		[SerializeField] private Settings settings;

		private SignalBus signalBus;
		private NavMeshAgent navMeshAgent;
		private Markers markers;
		private GameManager gameManager;
		private CharacterController3D characterController;

		[Inject]
        private void Construct(
			SignalBus signalBus,
			NavMeshAgent navMeshAgent,
			Markers markers,
			GameManager gameManager,
			CharacterController3D characterController)
		{
			this.signalBus = signalBus;
			this.navMeshAgent = navMeshAgent;
			this.markers = markers;
			this.gameManager = gameManager;
			this.characterController = characterController;
		}

		private void Update()
		{
			markers.TargetMarker.transform.position = CurrentNavMeshDestination;
			markers.TargetMarker.DrawCircle();

			markers.FollowMarker.DrawCircle();

			markers.LineMarker.DrawLine(navMeshAgent.path.corners);
		}

		public bool IsReachedDestination()
		{
			return (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance) && !navMeshAgent.pathPending;
		}
		public bool IsReachesDestination()
		{
			return (navMeshAgent.remainingDistance >= navMeshAgent.stoppingDistance) && !navMeshAgent.pathPending;
		}

		public bool SetTarget(Vector3 destination, float stoppingDistance = -1)
		{
			navMeshAgent.stoppingDistance = stoppingDistance <= 0 ? settings.reachTargetThreshold : stoppingDistance;

			if (IsPathValid(destination))
			{
				return navMeshAgent.SetDestination(destination);
			}

			return false;
		}

		public bool IsPathValid(Vector3 destination)
		{
			NavMeshPath path = new NavMeshPath();
			navMeshAgent.CalculatePath(destination, path);
			return path.status == NavMeshPathStatus.PathComplete;
		}


		private void Validate()
		{
			navMeshAgent.stoppingDistance = settings.reachTargetThreshold;
		}

		
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, settings.reachTargetThreshold);

			if (!Application.isPlaying) return;

			Vector3[] corners = navMeshAgent.path.corners;
			for (int i = 0; i < corners.Length - 1; i++)
			{
				Gizmos.DrawLine(corners[i], corners[i + 1]);
			}
		}

		[System.Serializable]
		public class Settings
		{
			public float reachTargetThreshold = 0.1f;
		}
	}
}