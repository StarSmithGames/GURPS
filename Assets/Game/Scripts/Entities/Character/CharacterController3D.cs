using CMF;

using Game.Entities;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Validation;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

using Zenject;

public class CharacterController3D : MonoBehaviour
{
	public UnityAction onTargetChanged;

	public bool IsGrounded => characterController.isGrounded;
	public bool IsHasTarget
	{
		get => isHasTarget;
		private set
		{
			if(isHasTarget != value)
			{
				isHasTarget = value;
			}
			onTargetChanged?.Invoke();
		}
	}
	private bool isHasTarget = false;
	public Vector3 CurrentDestination { get; private set; }

	public bool IsFreezed { get; private set; }
	public bool IsCanMove { get => isCanMove; set => isCanMove = value; }
	private bool isCanMove = true;

	[OnValueChanged("Validate", true)]
	[SerializeField] private Settings settings;

	private Vector3 lastPosition;
	private Vector3 lastVelocity;
	private Vector3 lastGravityVelocity;

	private float currentYRotation = 0f;
	private float fallOffAngle = 90f;
	private float magnitudeThreshold = 0.001f;

	private float timeOutTime = 1f;
	private float currentTimeOutTime = 1f;
	private float timeOutDistanceThreshold = 0.05f;

	private Transform model;

	private NavMeshAgent navMeshAgent;
	private CharacterController characterController;
	private NavigationController navigationController;

	[Inject]
	private void Construct(
		NavMeshAgent navMeshAgent,
		CharacterController characterController,
		NavigationController navigationController)
	{
		this.navMeshAgent = navMeshAgent;
		this.characterController = characterController;
		this.navigationController = navigationController;
	}

	private void Start()
	{
		Validate();
		model = transform;

		currentYRotation = model.localEulerAngles.y;
		CurrentDestination = transform.root.position;
	}


	private void OnAnimatorMove()
	{
		navMeshAgent.nextPosition = transform.root.position;
		//rootMotion += animator.deltaPosition;
		//root.position = animator.rootPosition;
		//model.rotation = animator.rootRotation;
	}

	private void FixedUpdate()
	{
		Movement();

		Rotation();

		//Handle timeout (stop controller if it is stuck);
		HandleTimeOut();
	}

	public void Freeze()
	{
		IsFreezed = true;

		if (IsHasTarget)
		{
			IsHasTarget = false;
			CurrentDestination = Vector3.zero;
		}
	}

	public void UnFreeze()
	{
		IsFreezed = false;
	}

	public bool SetDestination(Vector3 destination, float stoppingDistance = -1)
	{
		if (IsFreezed) return false;

		IsHasTarget = navigationController.SetTarget(destination, stoppingDistance);

		CurrentDestination = IsHasTarget ? destination : Vector3.zero;

		return IsHasTarget;
	}

	public Vector3 GetVelocity()
	{
		return lastVelocity;
	}


	private void Movement()
	{
		lastVelocity = CalculateMovementVelocity();
		characterController.Move(lastVelocity * settings.movementSpeed * Time.deltaTime);

		ApplyGravity();
	}

	private void ApplyGravity()
	{
		if (IsGrounded && lastGravityVelocity.y < 0)
		{
			lastGravityVelocity.y = 0f;
		}

		//if (Input.GetButtonDown("Jump")/* && groundedPlayer*/)
		//{
		//	lastGravityVelocity.y += Mathf.Sqrt(settings.jumpHeight * -3.0f * settings.gravity);
		//}

		lastGravityVelocity.y += settings.gravity * Time.deltaTime;
		characterController.Move(lastGravityVelocity * Time.deltaTime);
	}

	private void Rotation()
	{
		Vector3 velocity = lastVelocity;
		//Project velocity onto a plane defined by the 'up' direction of the parent transform;
		velocity = Vector3.ProjectOnPlane(velocity, transform.root.up);
		if (velocity.magnitude < magnitudeThreshold) return;
		velocity.Normalize();
		//Calculate (signed) angle between velocity and forward direction;
		float angleDifference = VectorMath.GetAngle(model.forward, velocity, model.up);
		//Calculate angle factor;
		float factor = Mathf.InverseLerp(0f, fallOffAngle, Mathf.Abs(angleDifference));
		//Calculate this frame's step;
		float step = Mathf.Sign(angleDifference) * factor * Time.deltaTime * settings.turnSpeed;
		if (angleDifference < 0f && step < angleDifference) step = angleDifference;
		else if (angleDifference > 0f && step > angleDifference) step = angleDifference;
		currentYRotation += step;
		if (currentYRotation > 360f) currentYRotation -= 360f;
		if (currentYRotation < -360f) currentYRotation += 360f;
		model.localRotation = Quaternion.Euler(0f, currentYRotation, 0f);
	}

	private Vector3 CalculateMovementVelocity()
	{
		if (!IsCanMove) return Vector3.zero;

		if (navigationController.IsReachedDestination() || !IsHasTarget)
		{
			IsHasTarget = false;
			//if (lastVelocity != Vector3.zero)
			//{
			//	lastVelocity = Vector3.MoveTowards(lastVelocity, Vector3.zero, 1f * Time.deltaTime);

			//	return lastVelocity;
			//}

			return Vector3.zero;
		}

		Vector3 direction = CurrentDestination - transform.root.position;
		direction = VectorMath.RemoveDotVector(direction, transform.root.up);
		Vector3 velocity = direction.normalized * settings.movementSpeed;

		return settings.useNavMesh ? navMeshAgent.desiredVelocity.normalized : velocity.normalized;
	}

	private void HandleTimeOut()
	{
		if (!IsHasTarget)
		{
			currentTimeOutTime = 0f;
			return;
		}

		if (Vector3.Distance(transform.root.position, lastPosition) > timeOutDistanceThreshold)
		{
			currentTimeOutTime = 0f;
			lastPosition = transform.root.position;
		}
		else
		{
			currentTimeOutTime += Time.deltaTime;

			//If current timeout time has reached limit, stop controller from moving;
			if (currentTimeOutTime >= timeOutTime)
			{
				IsHasTarget = false;
			}
		}
	}

	//-180 to 180
	public float CalculateAngleToDesination()
	{
		Vector3 desiredDiff = navMeshAgent.destination - model.position;
		Vector3 direction = Quaternion.Inverse(model.rotation) * desiredDiff.normalized;
		return Mathf.Atan2(direction.x, direction.z) * 180.0f / Mathf.PI;
	}


	private void Validate()
	{
		if (navMeshAgent == null)
		{
			if (Application.isEditor)
			{
				navMeshAgent = GetComponentInParent<NavMeshAgent>();
			}
		}

		navMeshAgent.updatePosition = false;
		navMeshAgent.speed = settings.movementSpeed;
		navMeshAgent.angularSpeed = 0;
	}

	private void OnDrawGizmos()
	{
		if (!IsHasTarget && !Application.isPlaying) return;

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(CurrentDestination, 0.1f);
	}

	[System.Serializable]
	public class Settings
	{
		public bool useNavMesh = true;
		[Space]
		public float movementSpeed = 5f;
		public AnimationCurve pathCurve;//eehh
		public float turnSpeed = 2000f;
		public float jumpHeight = 1.0f;
		public float gravity = -9.81f;
	}
}