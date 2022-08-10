using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class CharacterControllerRTS : MonoBehaviour
{
	public bool IsGrounded => characterController.isGrounded;

	[SerializeField] private Settings settings;

	private Vector3 lastPosition;
	private Vector3 lastVelocity;
	private Vector3 lastGravityVelocity;

	private CharacterController characterController;

	[Inject]
	private void Construct(CharacterController characterController)
	{
		this.characterController = characterController;
	}

	private void FixedUpdate()
	{
		Movement();
		ApplyGravity();

		//Rotation(lastVelocity);

		//Handle timeout (stop controller if it is stuck);
		//HandleTimeOut();
	}

	private void Movement()
	{
		//lastVelocity = CalculateMovementVelocity();
		//characterController.Move(lastVelocity * settings.movementSpeed * Time.deltaTime);
	}

	private void ApplyGravity()
	{
		if (IsGrounded && lastGravityVelocity.y < 0)
		{
			lastGravityVelocity.y = 0f;
		}

		lastGravityVelocity.y += settings.gravity * Time.deltaTime;
		characterController.Move(lastGravityVelocity * Time.deltaTime);
	}

	[System.Serializable]
	public class Settings
	{
		public float gravity = -9.81f;
	}
}