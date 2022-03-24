using Game.Managers.GameManager;
using Game.Systems.VFX;

using UnityEngine;

using Zenject;

namespace Game.Entities
{
	public class MarkerController : MonoBehaviour
	{
		[field: SerializeField] public LineRendererCircleVFX FollowMarker { get; private set; }
		[field: SerializeField] public LineRendererCircleVFX TargetMarker { get; private set; }
		[field: SerializeField] public LineRendererCircleVFX AreaMarker { get; private set; }
		[field: Space]
		[field: SerializeField] public LineRendererLineVFX LineMarker { get; private set; }


		private bool isHasTarget = false;

		private SignalBus signalBus;
		private NavigationController navigationController;
		private CharacterController3D characterController;
		private GameManager gameManager;

		[Inject]
		private void Construct(SignalBus signalBus, NavigationController navigationController, CharacterController3D characterController, GameManager gameManager)
		{
			this.signalBus = signalBus;
			this.navigationController = navigationController;
			this.characterController = characterController;
			this.gameManager = gameManager;

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

		private void Start()
		{
			FollowMarker.Enable(true);
			TargetMarker.Enable(false);
			AreaMarker.Enable(false);
		}

		private void FixedUpdate()
		{
			FollowMarker.DrawCircle();
			TargetMarker.DrawCircle();
			AreaMarker.DrawCircle();
		}

		private void OnTargetChanged()
		{
			if (characterController.IsHasTarget != isHasTarget)
			{
				isHasTarget = characterController.IsHasTarget;

				if (isHasTarget)
				{
					TargetMarker.transform.parent = null;
				}
				else
				{
					TargetMarker.transform.parent = transform;
				}
			}

			if (isHasTarget)
			{
				TargetMarker.transform.position = characterController.CurrentNavMeshDestination;

				if(gameManager.CurrentGameState == GameState.Battle)
				{
					LineMarker.DrawLine(navigationController.CurrentNavMeshPath);
				}
			}

			TargetMarker.Enable(isHasTarget);
		}

		private void OnGameStateChanged(SignalGameStateChanged signal)
		{
			LineMarker.Enable(signal.newGameState == GameState.Battle);
		}
	}
}