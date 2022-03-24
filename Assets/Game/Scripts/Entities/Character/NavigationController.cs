using UnityEngine;
using UnityEngine.AI;

using Zenject;

namespace Game.Entities
{
    public class NavigationController : MonoBehaviour
    {
		public Vector3[] CurrentNavMeshPath => navMeshAgent.path.corners;

		[SerializeField] private Settings settings;

		private NavMeshAgent navMeshAgent;

        [Inject]
        private void Construct(NavMeshAgent navMeshAgent)
		{
			this.navMeshAgent = navMeshAgent;
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
			navMeshAgent.stoppingDistance = stoppingDistance == -1 ? settings.reachTargetThreshold : settings.reachTargetThreshold;

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
			Gizmos.DrawWireSphere(transform.position, settings.reachTargetThreshold);

			if (!Application.isPlaying) return;

			Gizmos.color = Color.red;
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