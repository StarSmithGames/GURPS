using CMF;

using Game.Entities;
using Game.Managers.GameManager;

using UnityEngine;

using Zenject;

public class AnimatorControl : MonoBehaviour
{
	private int forwardSpeedHash;
	private int verticalSpeedHash;
	private int isBattleModeHash;
	private int isIdle;
	private int isGrounded;

	private SignalBus signalBus;
	private Animator animator;
	private Character character;
	private CharacterController3D controller;
	private GameManager gameManager;

	[Inject]
	private void Construct(SignalBus signalBus, Animator animator, Entity entity, GameManager gameManager, CharacterController3D controller)
	{
		this.signalBus = signalBus;
		this.animator = animator;
		this.character = entity as Character;
		this.gameManager = gameManager;
		this.controller = controller;
	}



	private void OnDestroy()
	{
		if(character != null)
		{
			character.onCharacterBattleStateChanged -= CheckBattleState;
		}
	}

	private void Start()
	{
		character.onCharacterBattleStateChanged += CheckBattleState;
		forwardSpeedHash = Animator.StringToHash("ForwardSpeed");
		verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
		isBattleModeHash = Animator.StringToHash("IsBattleMode");
		isIdle = Animator.StringToHash("IsIdle");
		isGrounded = Animator.StringToHash("IsGrounded");
	}

	private void Update()
	{
		Vector3 velocity = controller.GetVelocity();

		Vector3 horizontalVelocity = VectorMath.RemoveDotVector(velocity, transform.up);
		Vector3 verticalVelocity = velocity - horizontalVelocity;

		animator.SetFloat(forwardSpeedHash, velocity.magnitude);
		animator.SetFloat(verticalSpeedHash, verticalVelocity.magnitude * VectorMath.GetDotProduct(verticalVelocity, transform.up));
		//animator.SetFloat("HorizontalSpeed", Mathf.Clamp(controller.CalculateAngleToDesination(), -90, 90) / 90);
		animator.SetBool(isBattleModeHash, character.InBattle);

		animator.SetBool(isIdle, !controller.IsHasTarget && controller.IsGrounded && velocity.magnitude == 0);
		animator.SetBool(isGrounded, controller.IsGrounded);

		CheckBattleState();
	}

	private void CheckBattleState()
	{
		if (character.InBattle)
		{
			string animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

			AnimatorTransitionInfo transitionInfo = animator.GetAnimatorTransitionInfo(0);

			bool isIdleActionTransition = (transitionInfo.IsName("Idle -> IdleToIdleAction"));
			//transitionInfo.IsName("IdleActionToIdle -> IdleAction") ||
			//transitionInfo.IsName("IdleActionToIdle -> Idle"));

			bool isIdleAction = (animationName == "Armature|IdleToIdleAction");
			//animationName == "Armature|IdleAction" ||
			//animationName == "Armature|IdleActionToIdle");


			controller.IsCanMove = !isIdleAction && !isIdleActionTransition;
		}
		else
		{
			controller.IsCanMove = true;
		}
	}
}