using CMF;

using UnityEngine;

using Zenject;

public class AnimatorControl : MonoBehaviour
{
	//Whether the character is using the strafing blend tree;
	public bool useStrafeAnimations = false;

	//Velocity threshold for landing animation;
	//Animation will only be triggered if downward velocity exceeds this threshold;
	public float landVelocityThreshold = 5f;

	private float smoothingFactor = 40f;
	private Vector3 oldMovementVelocity = Vector3.zero;

	private Animator animator;
	private Mover controller;
	private SensorHandler sensor;

	[Inject]
	private void Construct(Animator animator, Mover controller, SensorHandler sensor)
	{
		this.animator = animator;
		this.controller = controller;
		this.sensor = sensor;
	}

	private void Update()
	{
		Vector3 velocity = controller.GetVelocity();

		Vector3 horizontalVelocity = VectorMath.RemoveDotVector(velocity, transform.up);
		Vector3 verticalVelocity = velocity - horizontalVelocity;

		animator.SetFloat("ForwardSpeed", velocity.normalized.magnitude);
		animator.SetFloat("VerticalSpeed", verticalVelocity.magnitude * VectorMath.GetDotProduct(verticalVelocity.normalized, transform.up));
		//animator.SetFloat("HorizontalSpeed", Mathf.Clamp(controller.CalculateAngleToDesination(), -90, 90) / 90);


		animator.SetBool("IsIdle", !controller.IsHasTarget && sensor.IsGrounded && velocity.normalized.magnitude == 0);
		animator.SetBool("IsGrounded", sensor.IsGrounded);
	}
}