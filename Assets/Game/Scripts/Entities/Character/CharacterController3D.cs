using CMF;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Validation;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Zenject;

public class CharacterController3D : MonoBehaviour
{
	public bool IsGrounded => controller.isGrounded;
	public bool IsHasTarget { get; private set; }
	public bool IsFreezed { get; private set; }

	[OnValueChanged("Validate", true)]
	[SerializeField] private Settings settings;

	private Vector3 lastPosition;
	private Vector3 lastVelocity;
	private Vector3 lastGravityVelocity;
	private Vector3 currentDestination;

	private float currentYRotation = 0f;
	private float fallOffAngle = 90f;
	private float magnitudeThreshold = 0.001f;

	private float timeOutTime = 1f;
	private float currentTimeOutTime = 1f;
	private float timeOutDistanceThreshold = 0.05f;


	private Transform model;

	private NavMeshAgent navMeshAgent;
	private CharacterController controller;

	[Inject]
	private void Construct(
		NavMeshAgent navMeshAgent,
		CharacterController controller)
	{
		this.navMeshAgent = navMeshAgent;
		this.controller = controller;
	}

	private void Start()
	{
		Validate();
		model = transform;

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
		if (IsFreezed) return;

		Movement();

		Rotation();

		//Handle timeout (stop controller if it is stuck);
		HandleTimeOut();
	}

	public void Freeze()
	{
		IsFreezed = true;
	}

	public void UnFreeze()
	{
		IsFreezed = false;
	}

	public Vector3 GetVelocityNormalized()
	{
		return lastVelocity.normalized;
	}


	private void Movement()
	{
		lastVelocity = CalculateMovementVelocity();
		controller.Move(lastVelocity * settings.movementSpeed * Time.deltaTime);

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
		controller.Move(lastGravityVelocity * Time.deltaTime);
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
		if (!IsHasTarget) return Vector3.zero;
		
		if (IsReachedDestination())
		{
			IsHasTarget = false;
			return Vector3.zero;
		}

		Vector3 direction = currentDestination - transform.root.position;
		direction = VectorMath.RemoveDotVector(direction, transform.root.up);
		Vector3 velocity = direction.normalized * settings.movementSpeed;

		return settings.useNavMesh ? navMeshAgent.desiredVelocity : velocity;
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

	#region Nav
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
		return SetDestination(destination, settings.reachTargetThreshold);
	}
	public bool SetDestination(Vector3 destination, float stoppingDistance)
	{
		navMeshAgent.stoppingDistance = stoppingDistance;
		return SetTarget(destination);
	}
	private bool SetTarget(Vector3 destination)
	{
		if (IsFreezed) return false;

		//NavMeshHit hit;
		//if (NavMesh.SamplePosition(destination, out hit, 1f, NavMesh.AllAreas))
		//{
		//	//Debug.LogError(IsPathValid(destination));

		//	IsHasTarget = navMeshAgent.SetDestination(destination);
		//	currentDestination = IsHasTarget ? destination : Vector3.zero;
		//	return IsHasTarget;
		//}

		if (IsPathValid(destination))
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
	#endregion

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
		navMeshAgent.stoppingDistance = settings.reachTargetThreshold;
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying) return;

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.root.position, settings.reachTargetThreshold);
		if (!IsHasTarget) return;
		Gizmos.DrawSphere(currentDestination, 0.1f);

		if (!settings.useNavMesh) return;

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