using Game.Entities;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

using Zenject;

namespace Game.Systems.NavigationSystem
{
	public class CharacterControllerRTS : MonoBehaviour, IController
	{
		public event UnityAction onReachedDestination;

		public Vector3 CurrentDestination { get; private set; }
		public bool IsHasTarget
		{
			get => isHasTarget;
			private set
			{
				if (isHasTarget != value)
				{
					isHasTarget = value;
				}
			}
		}
		private bool isHasTarget = false;
		public bool IsGrounded => characterController.isGrounded;

		[SerializeField] private Settings settings;

		private Vector3 lastPosition;
		private Vector3 lastVelocity;
		private Vector3 lastGravityVelocity;

		private CharacterController characterController;
		private NavigationController navigationController;

		[Inject]
		private void Construct(CharacterController characterController, NavigationController navigationController)
		{
			this.characterController = characterController;
			this.navigationController = navigationController;
		}

		private void Update()
		{
			Movement();
			ApplyGravity();

			//Rotation(lastVelocity);

			//Handle timeout (stop controller if it is stuck);
			//HandleTimeOut();
		}

		public void Freeze(bool trigger)
		{
		}

		public void Enable(bool trigger)
		{
		}

		public bool SetDestination(Vector3 destination, float maxPathDistance = -1)
		{
			//if (IsFreezed || IsWaitAnimation) return false;

			IsHasTarget = navigationController.SetTarget(destination, maxPathDistance);

			CurrentDestination = IsHasTarget ? navigationController.CurrentNavMeshDestination : Vector3.zero;

			return IsHasTarget;
		}

		public void Stop()
		{
			IsHasTarget = false;
			CurrentDestination = Vector3.zero;
		}

		public Vector3 GetVelocity()
		{
			return lastVelocity;
		}


		private void Movement()
		{
			lastVelocity = CalculateMovementVelocity();
			characterController.Move(lastVelocity * settings.movementSpeed * Time.deltaTime);
		}

		private void ApplyGravity()
		{
			if (IsGrounded && lastGravityVelocity.y <= 0)
			{
				lastGravityVelocity.y = 0f;
			}

			lastGravityVelocity.y += settings.gravity * Time.deltaTime;
			characterController.Move(lastGravityVelocity * Time.deltaTime);
		}

		private Vector3 CalculateMovementVelocity()
		{
			if (!IsHasTarget) return Vector3.zero;

			if (navigationController.NavMeshAgent.IsReachedDestination())
			{
				IsHasTarget = false;

				onReachedDestination?.Invoke();

				return Vector3.zero;
			}

			//if (!IsCanMove || IsWaitAnimation) return Vector3.zero;

			Vector3 direction = CurrentDestination - transform.root.position;
			direction = CMF.VectorMath.RemoveDotVector(direction, transform.root.up);
			Vector3 velocity = direction.normalized * settings.movementSpeed;

			return settings.useNavMesh ? navigationController.NavMeshAgent.desiredVelocity.normalized : velocity.normalized;
		}



		[System.Serializable]
		public class Settings
		{
			public bool useNavMesh = true;
			[Space]
			public float movementSpeed = 5f;
			public float gravity = -9.81f;
		}
	}
}