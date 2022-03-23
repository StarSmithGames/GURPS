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
		private CharacterController3D controller;
		private GameManager gameManager;

		[Inject]
		private void Construct(SignalBus signalBus, CharacterController3D controller, GameManager gameManager)
		{
			this.signalBus = signalBus;
			this.controller = controller;
			this.gameManager = gameManager;

			controller.onTargetChanged += OnTargetChanged;

			signalBus?.Subscribe<SignalGameStateChanged>(OnGameStateChanged);
		}

		private void OnDestroy()
		{
			if(controller != null)
			{
				controller.onTargetChanged -= OnTargetChanged;
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
			if (controller.IsHasTarget != isHasTarget)
			{
				isHasTarget = controller.IsHasTarget;

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
				TargetMarker.transform.position = controller.CurrentNavMeshDestination;

				if(gameManager.CurrentGameState == GameState.Battle)
				{
					LineMarker.DrawLine(controller.CurrentNavMeshPath);
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