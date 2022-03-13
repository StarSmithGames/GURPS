using CMF;

using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class PointClickController : BaseController
{
	[SerializeField] private Camera camera;
	[SerializeField] private NavMeshAgent navMeshAgent;
	[SerializeField] private Mover mover;

	[SerializeField] private Settings settings;
	
	public override bool IsGrounded { get; protected set; }

	private bool isHasTarget = false;

	private float currentTimeOutTime = 1f;
	private float currentVerticalSpeed = 0f;

	private float currentYRotation = 0f;
	[Tooltip("Если достигнут угл разварота в fallOffAngle, то turnSpeed стремится к 0. Эффект сглаживания к вращению.")]
	private float fallOffAngle = 90f;

	private float reachTargetThreshold = 0.001f;
	private float magnitudeThreshold = 0.001f;

	private Vector3 lastPosition;
	private Vector3 lastVelocity;
	private Vector3 lastMovementVelocity;
	private Vector3 currentDestination;

	protected Vector3 rootMotion;

	private void Start()
	{
		Assert.IsNotNull(camera);

		navMeshAgent.updatePosition = false;
		navMeshAgent.speed = settings.movementSpeed;
		navMeshAgent.angularSpeed = 0;
		navMeshAgent.stoppingDistance = reachTargetThreshold;

		lastPosition = transform.position;
		currentDestination = transform.position;

		currentYRotation = transform.localEulerAngles.y;
	}

	private void OnAnimatorMove()
	{
		navMeshAgent.nextPosition = transform.position;
	}

	//Mouse
	private void Update()
	{
		if (!settings.isCanHoldMouse && Mouse.IsMouseButtonDown() || settings.isCanHoldMouse && Mouse.IsMouseButtonPressed())
		{
			Ray mouseRay = camera.ScreenPointToRay(Mouse.GetMousePosition());

			if (Physics.Raycast(mouseRay, out RaycastHit hit, 100f, settings.raycastLayerMask, QueryTriggerInteraction.Ignore))
			{
				currentDestination = hit.point;

				isHasTarget = SetDestination(currentDestination);
			}
			else
			{
				isHasTarget = false;
			}
		}

		//if (!IsReachedDestination())
		//{
		//	controller.Move(navMeshAgent.desiredVelocity, false, false);
		//}
		//else
		//{
		//	controller.Move(Vector3.zero, false, false);
		//}
	}

	//Movement
	private void FixedUpdate()
	{
		mover.CheckForGround();
		IsGrounded = mover.IsGrounded;

		HandleTimeOut();

		//Calculate the final velocity for this frame;
		Vector3 velocity = CalculateMovementVelocity();
		lastMovementVelocity = velocity;

		ApplyGravity();
		velocity += transform.up * currentVerticalSpeed;

		//If the character is grounded, extend ground detection sensor range;
		mover.SetExtendSensorRange(IsGrounded);
		mover.SetVelocity(velocity);

		lastVelocity = velocity;
	}

	//Turn
	private void LateUpdate()
	{
		Vector3 velocity = settings.ignoreMomentum ? lastMovementVelocity : lastVelocity;

		//Project velocity onto a plane defined by the 'up' direction of the parent transform;
		velocity = Vector3.ProjectOnPlane(velocity, transform.up);

		if (velocity.magnitude < magnitudeThreshold) return;

		velocity.Normalize();

		//Calculate (signed) angle between velocity and forward direction;
		float angleDifference = VectorMath.GetAngle(transform.forward, velocity, transform.up);

		//Calculate angle factor;
		float factor = Mathf.InverseLerp(0f, fallOffAngle, Mathf.Abs(angleDifference));

		//Calculate this frame's step;
		float step = Mathf.Sign(angleDifference) * factor * Time.deltaTime * settings.turnSpeed;

		if (angleDifference < 0f && step < angleDifference) step = angleDifference;
		else if (angleDifference > 0f && step > angleDifference) step = angleDifference;

		currentYRotation += step;

		if (currentYRotation > 360f) currentYRotation -= 360f;
		if (currentYRotation < -360f) currentYRotation += 360f;


		transform.localRotation = Quaternion.Euler(0f, currentYRotation, 0f);
	}

	private Vector3 CalculateMovementVelocity()
	{
		if (!isHasTarget) return Vector3.zero;

		Vector3 direction = currentDestination - transform.position;

		//Remove all vertical parts of vector;
		direction = VectorMath.RemoveDotVector(direction, transform.up);

		float distanceToTarget = direction.magnitude;

		if (IsReachedDestination())
		{
			isHasTarget = false;
			return Vector3.zero;
		}

		Vector3 velocity = direction.normalized * settings.movementSpeed;

		//Check for overshooting;
		if (settings.movementSpeed * Time.fixedDeltaTime > distanceToTarget)
		{
			velocity = direction.normalized * distanceToTarget;
			isHasTarget = false;
		}

		return settings.useNavMesh ? navMeshAgent.desiredVelocity : velocity;
	}

	private void ApplyGravity()
	{
		if (!IsGrounded)
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
		if (!isHasTarget)
		{
			currentTimeOutTime = 0f;
			return;
		}

		//If controller has moved enough distance, reset time;
		if (Vector3.Distance(transform.position, lastPosition) > settings.timeOutDistanceThreshold)
		{
			currentTimeOutTime = 0f;
			lastPosition = transform.position;
		}
		else
		{
			currentTimeOutTime += Time.deltaTime;

			//If current timeout time has reached limit, stop controller from moving;
			if (currentTimeOutTime >= settings.timeOutTime)
			{
				isHasTarget = false;
			}
		}
	}


	public bool IsReachedDestination()
	{
		return (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance) && !navMeshAgent.pathPending;
	}
	public bool IsReachesDestination()
	{
		return (navMeshAgent.remainingDistance >= navMeshAgent.stoppingDistance) && !navMeshAgent.pathPending;
	}

	public bool SetDestination(Vector3 destination)
	{
		if (IsPathValid(destination))
		{
			navMeshAgent.SetDestination(destination);

			return true;
		}
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
		Vector3 desiredDiff = navMeshAgent.destination - transform.position;
		Vector3 direction = Quaternion.Inverse(transform.rotation) * desiredDiff.normalized;
		return Mathf.Atan2(direction.x, direction.z) * 180.0f / Mathf.PI;
	}


	public override Vector3 GetVelocity() => lastVelocity;
	public override Vector3 GetMovementVelocity() => lastMovementVelocity;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

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
		[Tooltip("Можно ли зажимать кнопку мыши")]
		public bool isCanHoldMouse = false;
		public bool ignoreMomentum = false;//Whether the current controller momentum should be ignored when calculating the new direction;
		[Space]
		public LayerMask raycastLayerMask = ~0;
		[Space]
		public float movementSpeed = 5f;
		public float turnSpeed = 2000f;
		public float gravity = 30f;
		[Space]
		[Tooltip("Если контроллер застрял при ходьбе у стены, движение будет отменено, если он не продвинулся хотя бы на определенное расстояние за определенное время")]
		public float timeOutTime = 1f;
		[Tooltip("This controls the minimum amount of distance needed to be moved (or else the controller stops moving)")]
		public float timeOutDistanceThreshold = 0.05f;
	}
}

public static class Mouse
{
	public static Vector2 GetMousePosition()
	{
		return Input.mousePosition;
	}

	public static bool IsMouseButtonDown()
	{
		return Input.GetMouseButtonDown(0);
	}
	public static bool IsMouseButtonPressed()
	{
		return Input.GetMouseButton(0);
	}
}