using CMF;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Zenject;

public class Mover : MonoBehaviour
{
	public bool IsHasTarget { get; protected set; }

	[SerializeField] private Settings settings;

	private float currentTimeOutTime = 1f;
	private float currentVerticalSpeed = 0f;

	private float currentYRotation = 0f;
	[Tooltip("Если достигнут угл разварота в fallOffAngle, то turnSpeed стремится к 0. Эффект сглаживания к вращению.")]
	private float fallOffAngle = 90f;

	private float magnitudeThreshold = 0.001f;

	private Vector3 lastPosition;
	private Vector3 lastVelocity;
	private Vector3 lastMovementVelocity;
	private Vector3 currentDestination;

	private Vector3 rootMotion;

	[Inject(Id ="Root")] private Transform root;
	[Inject(Id = "Model")] private Transform model;
	private Rigidbody rigidbody;
	private NavMeshAgent navMeshAgent;
	private Animator animator;
	private PointClickController controller;
	private SensorHandler sensor;

	[Inject]
	private void Construct(Rigidbody rigidbody, Animator animator, NavMeshAgent navMeshAgent, PointClickController controller, SensorHandler sensor)
	{
		this.rigidbody = rigidbody;
		this.animator = animator;
		this.navMeshAgent = navMeshAgent;
		this.controller = controller;
		this.sensor = sensor;
	}

	private void Start()
	{
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = false;

		navMeshAgent.updatePosition = false;
		navMeshAgent.speed = settings.movementSpeed;
		navMeshAgent.angularSpeed = 0;
		navMeshAgent.stoppingDistance = settings.reachTargetThreshold;

		currentYRotation = model.localEulerAngles.y;
	}

	private void OnAnimatorMove()
	{
		navMeshAgent.nextPosition = root.position;

		//rootMotion += animator.deltaPosition;

		//root.position = animator.rootPosition;
		//model.rotation = animator.rootRotation;
	}

	//Movement
	private void FixedUpdate()
	{
		HandleTimeOut();

		//Calculate the final velocity for this frame;
		Vector3 velocity = CalculateMovementVelocity();
		lastMovementVelocity = velocity;

		ApplyGravity();
		velocity += root.up * currentVerticalSpeed;

		rigidbody.velocity = velocity + sensor.CurrentGroundAdjustmentVelocity;

		rootMotion = Vector3.zero;

		lastVelocity = velocity;
	}

	//Turn
	private void LateUpdate()
	{
		Vector3 velocity = settings.ignoreMomentum ? lastMovementVelocity : lastVelocity;
		//Project velocity onto a plane defined by the 'up' direction of the parent transform;
		velocity = Vector3.ProjectOnPlane(velocity, root.up);

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

	public Vector3 GetVelocity() => lastVelocity;

	public bool IsReachedDestination()
	{
		return (navMeshAgent.remainingDistance < settings.reachTargetThreshold) && !navMeshAgent.pathPending;
	}
	public bool IsReachesDestination()
	{
		return (navMeshAgent.remainingDistance >= settings.reachTargetThreshold) && !navMeshAgent.pathPending;
	}

	public bool SetDestination(Vector3 destination)
	{
		if (destination != Vector3.zero && IsPathValid(destination))
		{
			IsHasTarget = navMeshAgent.SetDestination(destination);

			currentDestination = IsHasTarget ? destination : Vector3.zero;

			return IsHasTarget;
		}

		currentDestination = Vector3.zero;

		IsHasTarget = false;

		return false;
	}
	public bool IsPathValid(Vector3 destination)
	{
		NavMeshPath path = new NavMeshPath();
		navMeshAgent.CalculatePath(destination, path);

		return path.status == NavMeshPathStatus.PathComplete;
	}

	//-180 to 180
	public float CalculateAngleToDesination()
	{
		Vector3 desiredDiff = navMeshAgent.destination - model.position;
		Vector3 direction = Quaternion.Inverse(model.rotation) * desiredDiff.normalized;
		return Mathf.Atan2(direction.x, direction.z) * 180.0f / Mathf.PI;
	}


	private Vector3 CalculateMovementVelocity()
	{
		if (!IsHasTarget) return Vector3.zero;

		Vector3 direction = currentDestination - root.position;

		//Remove all vertical parts of vector;
		direction = VectorMath.RemoveDotVector(direction, root.up);

		float distanceToTarget = direction.magnitude;

		if (IsReachedDestination())
		{
			IsHasTarget = false;

			return Vector3.zero;
		}

		Vector3 velocity = direction.normalized * settings.movementSpeed;

		//Check for overshooting;
		if (settings.movementSpeed * Time.fixedDeltaTime > distanceToTarget)
		{
			velocity = direction.normalized * distanceToTarget;
			IsHasTarget = false;
		}

		return settings.useNavMesh ? navMeshAgent.desiredVelocity : velocity;
	}

	private void ApplyGravity()
	{
		if (!sensor.IsGrounded)
		{
			currentVerticalSpeed -= settings.gravity * Time.deltaTime;
		}
		else
		{
			if (currentVerticalSpeed < 0f)
			{
				//if (OnLand != null)
				//	OnLand(tr.up * currentVerticalSpeed);
			}

			currentVerticalSpeed = 0f;
		}
	}

	//Handle timeout (stop controller if it is stuck);
	private void HandleTimeOut()
	{
		if (!IsHasTarget)
		{
			currentTimeOutTime = 0f;
			return;
		}

		//If controller has moved enough distance, reset time;
		if (Vector3.Distance(root.position, lastPosition) > settings.timeOutDistanceThreshold || controller.IsMouseHolded)
		{
			currentTimeOutTime = 0f;
			lastPosition = root.position;
		}
		else
		{
			currentTimeOutTime += Time.deltaTime;

			//If current timeout time has reached limit, stop controller from moving;
			if (currentTimeOutTime >= settings.timeOutTime)
			{
				IsHasTarget = false;
			}
		}
	}


	private void OnDrawGizmos()
	{
		if (!Application.isPlaying) return;

		Gizmos.color = Color.red;

		Gizmos.DrawWireSphere(root.position, settings.reachTargetThreshold);

		if (!IsHasTarget) return;

		Gizmos.DrawSphere(currentDestination, 0.1f);

		Vector3[] corners = navMeshAgent.path.corners;

		for (int i = 0; i < corners.Length - 1; i++)
		{
			Gizmos.DrawLine(corners[i], corners[i + 1]);
		}
	}


	[System.Serializable]
	public class Settings
	{
		public bool useNavMesh = true;
		public bool ignoreMomentum = false;//Whether the current controller momentum should be ignored when calculating the new direction;
		[Space]
		public float movementSpeed = 5f;
		public float turnSpeed = 2000f;
		public float gravity = 30f;
		[Space]
		public float reachTargetThreshold = 0.1f;

		[Tooltip("Если контроллер застрял при ходьбе у стены, движение будет отменено, если он не продвинулся хотя бы на определенное расстояние за определенное время")]
		public float timeOutTime = 1f;
		[Tooltip("This controls the minimum amount of distance needed to be moved (or else the controller stops moving)")]
		public float timeOutDistanceThreshold = 0.05f;
	}
}