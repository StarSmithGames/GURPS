using CMF;

using Game.Managers.GameManager;
using Game.Systems.BattleSystem;

using UnityEngine;

using Zenject;

public class AnimatorControl : MonoBehaviour
{
	private SignalBus signalBus;
	private Animator animator;
	private CharacterController3D controller;
	private GameManager gameManager;

	[Inject]
	private void Construct(SignalBus signalBus, Animator animator, GameManager gameManager, CharacterController3D controller)
	{
		this.signalBus = signalBus;
		this.animator = animator;
		this.gameManager = gameManager;
		this.controller = controller;
	}

	private void OnDestroy()
	{
		signalBus?.Unsubscribe<SignalCurrentBattleChanged>(OnCurrentBattleChanged);
	}

	private void Start()
	{
		signalBus?.Subscribe<SignalCurrentBattleChanged>(OnCurrentBattleChanged);
	}

	private void Update()
	{
		Vector3 velocity = controller.GetVelocityNormalized();

		Vector3 horizontalVelocity = VectorMath.RemoveDotVector(velocity, transform.up);
		Vector3 verticalVelocity = velocity - horizontalVelocity;

		animator.SetFloat("ForwardSpeed", velocity.magnitude);
		animator.SetFloat("VerticalSpeed", verticalVelocity.magnitude * VectorMath.GetDotProduct(verticalVelocity, transform.up));
		//animator.SetFloat("HorizontalSpeed", Mathf.Clamp(controller.CalculateAngleToDesination(), -90, 90) / 90);
		animator.SetBool("IsBattleMode", transform.root.GetComponent<IBattlable>().InBattle);//stub

		animator.SetBool("IsIdle", !controller.IsHasTarget && controller.IsGrounded && velocity.magnitude == 0);
		animator.SetBool("IsGrounded", controller.IsGrounded);
	}

	private void OnCurrentBattleChanged(SignalCurrentBattleChanged signal)
	{
	}
}