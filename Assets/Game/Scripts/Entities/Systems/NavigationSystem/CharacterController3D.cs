using CMF;

using DG.Tweening;

using Game.Entities;

using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

using Zenject;

public class CharacterController3D : MonoBehaviour, IController
{
	public event UnityAction onReachedDestination;

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
		}
	}
	private bool isHasTarget = false;

	public bool IsFreezed { get; private set; }
	public bool IsWaitAnimation { get; set; }

	public bool IsCanMove { get => isCanMove; set => isCanMove = value; }
	private bool isCanMove = true;

	public bool IsCanRotate { get => isCanRotate; set => isCanRotate = value; }
	private bool isCanRotate = true;

	public Vector3 CurrentDestination { get; private set; }

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

	private CharacterController characterController;
	private NavigationController navigationController;

	[Inject]
	private void Construct(CharacterController characterController, NavigationController navigationController)
	{
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


	private void Update()
	{
		Movement();

		Rotation(lastVelocity);

		//Handle timeout (stop controller if it is stuck);
		HandleTimeOut();
	}

	public void Freeze(bool trigger)
	{
		IsFreezed = trigger;

		if (IsFreezed && IsHasTarget)
		{
			Stop();
		}
	}

	public void Enable(bool trigger)
	{
		navigationController.NavMeshAgent.enabled = trigger;
		characterController.enabled = trigger;
		navigationController.enabled = trigger;

		enabled = trigger;
	}

	public bool SetDestination(Vector3 destination, float maxPathDistance = -1)
	{
		if (IsFreezed || IsWaitAnimation) return false;

		IsHasTarget = navigationController.SetTarget(destination, maxPathDistance);

		CurrentDestination = IsHasTarget ? navigationController.CurrentNavMeshDestination : Vector3.zero;

		return IsHasTarget;
	}

	public void Stop()
	{
		IsHasTarget = false;
		CurrentDestination = Vector3.zero;
	}

	public void RotateTo(Vector3 point)
	{
		Rotation((point - model.position).normalized);
	}

	public Tween RotateAnimatedTo(Vector3 point, float duration)
	{
		var lookPos = (point - model.position).normalized;
		var rotation = Quaternion.LookRotation(lookPos, Vector3.up);

		return model.DORotate(rotation.eulerAngles, duration);
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

	private void Rotation(Vector3 direction)
	{
		if (!IsCanRotate) return;
		if (direction == Vector3.zero) return;

		Vector3 velocity = direction;
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
		if (!IsHasTarget) return Vector3.zero;

		if (navigationController.NavMeshAgent.IsReachedDestination())
		{
			IsHasTarget = false;

			onReachedDestination?.Invoke();

			return Vector3.zero;
		}

		if (!IsCanMove || IsWaitAnimation) return Vector3.zero;

		Vector3 direction = CurrentDestination - transform.root.position;
		direction = VectorMath.RemoveDotVector(direction, transform.root.up);
		Vector3 velocity = direction.normalized * settings.movementSpeed;

		return settings.useNavMesh ? navigationController.NavMeshAgent.desiredVelocity.normalized : velocity.normalized;
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
		Vector3 desiredDiff = navigationController.NavMeshAgent.destination - model.position;
		Vector3 direction = Quaternion.Inverse(model.rotation) * desiredDiff.normalized;
		return Mathf.Atan2(direction.x, direction.z) * 180.0f / Mathf.PI;
	}


	private void Validate()
	{
		navigationController.NavMeshAgent.updatePosition = false;
		navigationController.NavMeshAgent.speed = settings.movementSpeed;
		navigationController.NavMeshAgent.angularSpeed = 0;
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