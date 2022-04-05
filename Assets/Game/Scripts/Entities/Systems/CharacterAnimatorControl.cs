using CMF;

using DG.Tweening;
using Game.Entities;

using Sirenix.OdinInspector;

using System.Collections;

using UnityEngine;

using Zenject;

public class CharacterAnimatorControl : MonoBehaviour
{
	public bool IsRootMotion
	{
		get => isRootMotion;
		private set
		{
			isRootMotion = value;

			controller.IsCanMove = !isRootMotion;
			controller.IsCanRotate = !isRootMotion;
		}
	}
	private bool isRootMotion = false;
	private bool isAttackProccess = false;
	private bool isWaitAnimationProccess = false;
	private bool isWaitTransitionProccess = false;

	private int forwardSpeedHash;
	private int verticalSpeedHash;
	private int isBattleModeHash;
	private int isIdle;
	private int isGrounded;
	private int weaponType;
	private int attack;

	private SignalBus signalBus;
	private Animator animator;
	private Character character;
	private CharacterController3D controller;
	private CharacterOutfit outfit;
	
	[Inject]
	private void Construct(
		SignalBus signalBus,
		Animator animator,
		Entity entity,
		CharacterController3D controller,
		CharacterOutfit outfit)
	{
		this.signalBus = signalBus;
		this.animator = animator;
		this.character = entity as Character;
		this.controller = controller;
		this.outfit = outfit;
	}

	private void OnDestroy()
	{
		if(character != null)
		{
			character.onDestinationChanged -= OnDestinationChanged;
			controller.onReachedDestination -= OnReachedDestination;
		}
	}

	private void Start()
	{
		animator.applyRootMotion = false;

		character.onDestinationChanged += OnDestinationChanged;
		controller.onReachedDestination += OnReachedDestination;

		forwardSpeedHash = Animator.StringToHash("ForwardSpeed");
		verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
		isBattleModeHash = Animator.StringToHash("IsBattleMode");
		isIdle = Animator.StringToHash("IsIdle");
		isGrounded = Animator.StringToHash("IsGrounded");
		weaponType = Animator.StringToHash("WeaponType");
		attack = Animator.StringToHash("Attack");
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

		animator.SetBool(isIdle, !controller.IsHasTarget && velocity.magnitude == 0);
		animator.SetBool(isGrounded, controller.IsGrounded);

		animator.SetInteger(weaponType, (int)outfit.CurrentWeaponType);


		if (animator.applyRootMotion == false)
		{
			character.Navigation.NavMeshAgent.nextPosition = transform.root.position;
		}

		BattleActions();
	}

	

	public void Hit()
	{
		animator.SetTrigger("Hit");
	}

	public void Attack()
	{
		StartCoroutine(AttackProccess());
	}

	private void BattleActions()
	{
		if (character.InBattle)
		{
			controller.IsWaitAnimation = isAttackProccess || isWaitAnimationProccess || isWaitTransitionProccess;
		}
	}

	private IEnumerator AttackProccess()
	{
		isAttackProccess = true;

		yield return WaitWhileAnimation("Armature|IdleAction");

		animator.applyRootMotion = true;

		animator.SetTrigger(attack);
		
		while (true)
		{
			string animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

			transform.rotation = animator.rootRotation;

			if (animationName == "IdleFightToActionIdle")
			{
				break;
			}

			yield return null;
		}

		transform.DOMove(transform.root.position, 0.25f);

		yield return WaitWhileAnimation("Armature|IdleAction");

		animator.applyRootMotion = false;
		isAttackProccess = false;
	}


	private IEnumerator WaitWhileAnimation(string animation)
	{
		isWaitAnimationProccess = true;

		while (character.InBattle)
		{
			if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == animation)
			{
				break;
			}
			yield return null;
		}

		isWaitAnimationProccess = false;
	}

	private IEnumerator WaitWhileTransition(string transition)
	{
		isWaitTransitionProccess = true;

		while (character.InBattle)
		{
			if (animator.GetAnimatorTransitionInfo(0).IsName(transition))
			{
				break;
			}
			
			yield return null;
		}

		isWaitTransitionProccess = false;
	}


	private void OnDestinationChanged()
	{

	}

	private void OnReachedDestination()
	{
		if (character.InBattle)
		{
			StartCoroutine(WaitWhileAnimation("Armature|IdleAction"));
		}
	}


	#region AnimationEvents
	private void HitEvent()
	{
		Debug.LogError("HERE");
	}
	#endregion
}