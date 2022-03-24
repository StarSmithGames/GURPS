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
		private Markers markerController;
		private GameManager gameManager;
		private CharacterController3D characterController;

		[Inject]
        private void Construct(
			SignalBus signalBus,
			NavMeshAgent navMeshAgent,
			Markers markerController,
			GameManager gameManager,
			CharacterController3D characterController)
		{
			this.signalBus = signalBus;
			this.navMeshAgent = navMeshAgent;
			this.markerController = markerController;
			this.gameManager = gameManager;
			this.characterController = characterController;

			characterController.onTargetChanged += OnTargetChanged;

			signalBus?.Subscribe<SignalGameStateChanged>(OnGameStateChanged);
		}

		private void OnDestroy()
		{
			if(characterController != null)
			{
				characterController.onTargetChanged -= OnTargetChanged;
			}

			signalBus?.Unsubscribe<SignalGameStateChanged>(OnGameStateChanged);
		}

		private void Update()
		{
			markerController.TargetMarker.transform.position = CurrentNavMeshDestination;
			markerController.TargetMarker.DrawCircle();

			markerController.FollowMarker.DrawCircle();

			markerController.LineMarker.DrawLine(navMeshAgent.path.corners);
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

		private void OnTargetChanged()
		{
			if (gameManager.CurrentGameState == GameState.Gameplay)
			{
				if (characterController.IsHasTarget)
				{
					if (!markerController.TargetMarker.IsEnabled)
					{
						markerController.TargetMarker.EnableIn();
					}
				}
				else
				{
					if (markerController.TargetMarker.IsEnabled)
					{
						markerController.TargetMarker.EnableOut();
					}
				}
			}
		}

		private void OnGameStateChanged(SignalGameStateChanged signal)
		{
			if(signal.newGameState == GameState.Gameplay)
			{
				markerController.FollowMarker.Enable(false);

				markerController.TargetMarker.transform.parent = null;
				markerController.TargetMarker.Enable(false);

				markerController.AreaMarker.Enable(false);

				markerController.LineMarker.Enable(false);
			}
			else if(signal.newGameState == GameState.PreBattle)
			{
				markerController.FollowMarker.Enable(false);

				markerController.TargetMarker.Enable(false);

				markerController.AreaMarker.Enable(false);

				markerController.LineMarker.Enable(false);
			}
			else if(signal.newGameState == GameState.Battle)
			{
				markerController.FollowMarker.Enable(true);

				markerController.TargetMarker.transform.parent = null;
				markerController.TargetMarker.Enable(true);

				markerController.AreaMarker.Enable(false);

				markerController.LineMarker.Enable(true);
			}
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