using CMF;

using Game.Entities;
using Game.Systems.BattleSystem;

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using Zenject;

public class AnimatorControl : MonoBehaviour
{
	public UnityAction onAttackEvent;

	protected bool isAttackProccess = false;
	protected bool isWaitAnimationProccess = false;
	protected bool isWaitTransitionProccess = false;

	protected int forwardSpeedHash;
	protected int verticalSpeedHash;
	protected int isBattleModeHash;
	protected int isIdleHash;
	protected int isGroundedHash;
	protected int attackHash;
	protected int hitHash;
	protected int hitTypeHash;

	protected Animator animator;
	protected Entity entity;

	[Inject]
	private void Construct(
		Animator animator,
		Entity entity)
	{
		this.animator = animator;
		this.entity = entity;
	}

	protected virtual void OnDestroy()
	{
		if (entity != null)
		{
			entity.onDestinationChanged -= OnDestinationChanged;
			entity.Controller.onReachedDestination -= OnReachedDestination;
		}
	}

	protected virtual void Start()
	{
		animator.applyRootMotion = false;

		forwardSpeedHash = Animator.StringToHash("ForwardSpeed");
		verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
		isBattleModeHash = Animator.StringToHash("IsBattleMode");
		isIdleHash = Animator.StringToHash("IsIdle");
		isGroundedHash = Animator.StringToHash("IsGrounded");

		attackHash = Animator.StringToHash("Attack");
		hitHash = Animator.StringToHash("Hit");
		hitTypeHash = Animator.StringToHash("HitType");

		entity.onDestinationChanged += OnDestinationChanged;
		entity.Controller.onReachedDestination += OnReachedDestination;
	}

	protected virtual void Update()
{
		Vector3 velocity = entity.Controller.GetVelocity();

		Vector3 horizontalVelocity = VectorMath.RemoveDotVector(velocity, transform.up);
		Vector3 verticalVelocity = velocity - horizontalVelocity;

		animator.SetFloat(forwardSpeedHash, velocity.magnitude);
		animator.SetFloat(verticalSpeedHash, verticalVelocity.magnitude * VectorMath.GetDotProduct(verticalVelocity, transform.up));
		//animator.SetFloat("HorizontalSpeed", Mathf.Clamp(controller.CalculateAngleToDesination(), -90, 90) / 90);

		animator.SetBool(isIdleHash, !entity.Controller.IsHasTarget && velocity.magnitude == 0);
		animator.SetBool(isGroundedHash, entity.Controller.IsGrounded);


		if (animator.applyRootMotion == false)
		{
			entity.Navigation.NavMeshAgent.nextPosition = transform.root.position;
		}
	}


	public virtual void Hit(int type = -1)
	{
		animator.SetInteger(hitTypeHash, type == -1 ? 0 : type);
		animator.SetTrigger("Hit");
	}
	public virtual void Attack(int type = -1) { }

	protected IEnumerator WaitWhileAnimation(string animation)
	{
		if(entity is IBattlable battlable)
		{
			isWaitAnimationProccess = true;

			while (battlable.InBattle)
			{
				if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == animation)
				{
					break;
				}
				yield return null;
			}

			isWaitAnimationProccess = false;
		}
		
	}

	protected IEnumerator WaitWhileTransition(string transition)
	{
		if (entity is IBattlable battlable)
		{
			isWaitTransitionProccess = true;

			while (battlable.InBattle)
			{
				if (animator.GetAnimatorTransitionInfo(0).IsName(transition))
				{
					break;
				}

				yield return null;
			}

			isWaitTransitionProccess = false;
		}
	}


	private void OnDestinationChanged() { }

	private void OnReachedDestination()
	{
		if(entity is IBattlable battlable)
		{
			if (battlable.InBattle)
			{
				StartCoroutine(WaitWhileAnimation("Armature|IdleAction"));
			}
		}
	}


	#region AnimationEvents
	private void AttackEvent()
	{
		onAttackEvent?.Invoke();
	}
	#endregion
}