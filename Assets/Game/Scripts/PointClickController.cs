using CMF;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PointClickController : MonoBehaviour
{
	public Camera playerCamera;
	public Mover mover;
	[Space]
	[Tooltip("Можно ли зажимать кнопку мыши")]
	public bool isCanHoldMouse = false;
	[Space]
	public float movementSpeed = 10f;
	public float gravity = 30f;

	public MouseDetectionType mouseDetectionType = MouseDetectionType.AbstractPlane;
	public LayerMask raycastLayerMask = ~0;

	private float currentVerticalSpeed = 0f;
	private bool isGrounded = false;

	[Tooltip("Если контроллер застрял при ходьбе у стены, движение будет отменено, если он не продвинулся хотя бы на определенное расстояние за определенное время")]
	public float timeOutTime = 1f;
	//This controls the minimum amount of distance needed to be moved (or else the controller stops moving);
	public float timeOutDistanceThreshold = 0.05f;

	private float currentTimeOutTime = 1f;

	private Vector3 lastPosition;
	public Vector3 LastVelocity { get; private set; }
	public Vector3 LastMovementVelocity { get; private set; }
	private Vector3 currentTargetPosition;
	private bool hasTarget = false;

	private float reachTargetThreshold = 0.001f;

	private Plane groundPlane;

	private void Start()
	{
		Assert.IsNotNull(playerCamera);
		Assert.IsNotNull(mover);

		lastPosition = transform.position;
		currentTargetPosition = transform.position;
		groundPlane = new Plane(transform.up, transform.position);
	}

	private void Update()
	{
		HandleMouseInput();
	}

	private void FixedUpdate()
	{
		//Run initial mover ground check;
		mover.CheckForGround();

		//Check whether the character is grounded;
		isGrounded = mover.IsGrounded();

		//Handle timeout (stop controller if it is stuck);
		HandleTimeOut();

		Vector3 _velocity = Vector3.zero;

		//Calculate the final velocity for this frame;
		_velocity = CalculateMovementVelocity();
		LastMovementVelocity = _velocity;

		//Calculate and apply gravity;
		HandleGravity();
		_velocity += transform.up * currentVerticalSpeed;

		//If the character is grounded, extend ground detection sensor range;
		mover.SetExtendSensorRange(isGrounded);
		//Set mover velocity;
		mover.SetVelocity(_velocity);

		//Save velocity for later;
		LastVelocity = _velocity;
	}

	//Calculate movement velocity based on the current target position;
	private Vector3 CalculateMovementVelocity()
	{
		//Return no velocity if controller currently has no target;	
		if (!hasTarget)
			return Vector3.zero;

		//Calculate vector to target position;
		Vector3 _toTarget = currentTargetPosition - transform.position;

		//Remove all vertical parts of vector;
		_toTarget = VectorMath.RemoveDotVector(_toTarget, transform.up);

		//Calculate distance to target;
		float _distanceToTarget = _toTarget.magnitude;

		//If controller has already reached target position, return no velocity;
		if (_distanceToTarget <= reachTargetThreshold)
		{
			hasTarget = false;
			return Vector3.zero;
		}

		Vector3 _velocity = _toTarget.normalized * movementSpeed;

		//Check for overshooting;
		if (movementSpeed * Time.fixedDeltaTime > _distanceToTarget)
		{
			_velocity = _toTarget.normalized * _distanceToTarget;
			hasTarget = false;
		}

		return _velocity;
	}

	//Calculate current gravity;
	private void HandleGravity()
	{
		//Handle gravity;
		if (!isGrounded)
			currentVerticalSpeed -= gravity * Time.deltaTime;
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

	private void HandleMouseInput()
	{
		//If no camera has been assigned, stop function execution;
		if (playerCamera == null)
			return;

		//If a valid mouse press has been detected, raycast to determine the new target position;
		if (!isCanHoldMouse && Mouse.IsMouseButtonDown() || isCanHoldMouse && Mouse.IsMouseButtonPressed())
		{
			//Set up mouse ray (based on screen position);
			Ray _mouseRay = playerCamera.ScreenPointToRay(Mouse.GetMousePosition());

			if (mouseDetectionType == MouseDetectionType.AbstractPlane)
			{
				//Set up abstract ground plane;
				groundPlane.SetNormalAndPosition(transform.up, transform.position);
				float _enter = 0f;

				//Raycast against ground plane;
				if (groundPlane.Raycast(_mouseRay, out _enter))
				{
					currentTargetPosition = _mouseRay.GetPoint(_enter);
					hasTarget = true;
				}
				else
					hasTarget = false;
			}
			else if (mouseDetectionType == MouseDetectionType.Raycast)
			{
				RaycastHit _hit;

				//Raycast against level geometry;
				if (Physics.Raycast(_mouseRay, out _hit, 100f, raycastLayerMask, QueryTriggerInteraction.Ignore))
				{
					currentTargetPosition = _hit.point;
					hasTarget = true;
				}
				else
					hasTarget = false;
			}
		}
	}

	//Handle timeout (stop controller from moving if it is stuck against level geometry);
	private void HandleTimeOut()
	{
		//If controller currently has no target, reset time and return;
		if (!hasTarget)
		{
			currentTimeOutTime = 0f;
			return;
		}

		//If controller has moved enough distance, reset time;
		if (Vector3.Distance(transform.position, lastPosition) > timeOutDistanceThreshold)
		{
			currentTimeOutTime = 0f;
			lastPosition = transform.position;
		}
		//If controller hasn't moved a sufficient distance, increment current timeout time;
		else
		{
			currentTimeOutTime += Time.deltaTime;

			//If current timeout time has reached limit, stop controller from moving;
			if (currentTimeOutTime >= timeOutTime)
			{
				hasTarget = false;
			}
		}
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


//Whether the target position is determined by raycasting against an abstract plane or the actual level geometry;
//'AbstractPlane' is less accurate, but simpler (and will automatically ignore colliders between the camera and target position);
//'Raycast' is more accurate, but ceilings or intersecting geometry (between camera and target position) must be handled separately;
public enum MouseDetectionType
{
	AbstractPlane,
	Raycast
}
