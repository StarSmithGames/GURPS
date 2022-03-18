using CMF;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Zenject;

public class CharacterThirdPersonController : MonoBehaviour
{
	public bool IsHasTarget { get; private set; }

	[SerializeField] private Settings settings;

	private Vector3 lastVelocity;
	private Vector3 currentDestination;

	private float currentYRotation = 0f;
	private float fallOffAngle = 90f;
	private float magnitudeThreshold = 0.001f;

	private Transform model;

	private NavMeshAgent navMeshAgent;
	private Animator animator;
	private PointClickInput input;
	private CharacterController controller;

	[Inject]
	private void Construct(
		Animator animator,
		NavMeshAgent navMeshAgent,
		PointClickInput input,
		CharacterController controller)
	{
		this.animator = animator;
		this.navMeshAgent = navMeshAgent;
		this.input = input;
		this.controller = controller;

		model = transform;
	}

	private void Start()
	{
		navMeshAgent.updatePosition = false;
		navMeshAgent.speed = settings.movementSpeed;
		navMeshAgent.angularSpeed = 0;
		navMeshAgent.stoppingDistance = settings.reachTargetThreshold;

		currentYRotation = model.localEulerAngles.y;
		currentDestination = transform.root.position;
	}

	private void OnAnimatorMove()
	{
		navMeshAgent.nextPosition = transform.root.position;
		//rootMotion += animator.deltaPosition;
		//root.position = animator.rootPosition;
		//model.rotation = animator.rootRotation;
	}

	private void Update()
	{
		ApplyGravity();
		if (controller.isGrounded && lastVelocity.y < 0)
		{
			lastVelocity.y = 0f;
		}

		//if (Input.GetButtonDown("Jump")/* && groundedPlayer*/)
		//{
		//	lastVelocity.y += Mathf.Sqrt(settings.jumpHeight * -3.0f * settings.gravity);
		//}

		lastVelocity.y += settings.gravity * Time.deltaTime;
		controller.Move(lastVelocity * Time.deltaTime);

		Movement();

		Rotation();
	}

	private void Movement()
	{
		lastVelocity = CalculateMovementVelocity();
		if (!controller.isGrounded)
		{
			lastVelocity += Physics.gravity;
		}
		controller.Move(lastVelocity * settings.movementSpeed * Time.deltaTime);
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

	private void ApplyGravity()
	{

	}

	private Vector3 CalculateMovementVelocity()
	{
		if (!IsHasTarget) return Vector3.zero;
		
		if (IsReachedDestination())
		{
			IsHasTarget = false;
			return Vector3.zero;
		}

		Vector3 direction = currentDestination - transform.root.position;
		direction = VectorMath.RemoveDotVector(direction, transform.root.up);
		Vector3 velocity = direction.normalized * settings.movementSpeed;
		//Check for overshooting;
		//if (settings.movementSpeed * Time.fixedDeltaTime > distanceToTarget)
		//{
		//	velocity = direction.normalized * distanceToTarget;
		//	IsHasTarget = false;
		//}
		return settings.useNavMesh ? navMeshAgent.desiredVelocity : velocity;
	}


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
			//target.position = currentDestination;
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



	private void OnDrawGizmos()
	{
		if (!Application.isPlaying) return;

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.root.position, settings.reachTargetThreshold);
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
		[Space]
		public float movementSpeed = 5f;
		public float turnSpeed = 2000f;
		public float jumpHeight = 1.0f;
		public float gravity = -9.81f;

		[Space]
		public float reachTargetThreshold = 0.1f;
	}
}