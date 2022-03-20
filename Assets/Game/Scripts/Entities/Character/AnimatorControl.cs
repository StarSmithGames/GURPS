using CMF;

using UnityEngine;

using Zenject;

public class AnimatorControl : MonoBehaviour
{
	private Animator animator;
	private CharacterController3D controller;

	[Inject]
	private void Construct(Animator animator, CharacterController3D controller)
	{
		this.animator = animator;
		this.controller = controller;
	}

	private void Update()
	{
		Vector3 velocity = controller.GetVelocityNormalized();

		Vector3 horizontalVelocity = VectorMath.RemoveDotVector(velocity, transform.up);
		Vector3 verticalVelocity = velocity - horizontalVelocity;

		animator.SetFloat("ForwardSpeed", velocity.magnitude);
		animator.SetFloat("VerticalSpeed", verticalVelocity.magnitude * VectorMath.GetDotProduct(verticalVelocity, transform.up));
		//animator.SetFloat("HorizontalSpeed", Mathf.Clamp(controller.CalculateAngleToDesination(), -90, 90) / 90);

		animator.SetBool("IsIdle", !controller.IsHasTarget && controller.IsGrounded && velocity.magnitude == 0);
		animator.SetBool("IsGrounded", controller.IsGrounded);
	}
}