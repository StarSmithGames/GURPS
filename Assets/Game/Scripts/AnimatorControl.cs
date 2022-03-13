using CMF;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControl : MonoBehaviour
{
	[SerializeField] private Animator animator;
	[SerializeField] private BaseController controller;

	//Whether the character is using the strafing blend tree;
	public bool useStrafeAnimations = false;

	//Velocity threshold for landing animation;
	//Animation will only be triggered if downward velocity exceeds this threshold;
	public float landVelocityThreshold = 5f;

	private float smoothingFactor = 40f;
	Vector3 oldMovementVelocity = Vector3.zero;

	private void Update()
	{
		Vector3 velocity = controller.GetVelocity();

		Vector3 horizontalVelocity = VectorMath.RemoveDotVector(velocity, transform.up);
		Vector3 verticalVelocity = velocity - horizontalVelocity;
		Vector3 localVelocity = transform.InverseTransformVector(horizontalVelocity);

		animator.SetFloat("ForwardSpeed", Mathf.Abs(velocity.normalized.z));
		animator.SetFloat("VerticalSpeed", verticalVelocity.magnitude * VectorMath.GetDotProduct(verticalVelocity.normalized, transform.up));

		animator.SetBool("IsIdle", velocity.normalized.magnitude == 0);
		animator.SetBool("IsGrounded", controller.IsGrounded);
	}
}