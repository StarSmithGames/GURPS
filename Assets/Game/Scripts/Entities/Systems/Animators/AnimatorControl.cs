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

	public bool IsRootMotion { get; private set; }
	public bool IsIdle { get; private set; }

	public virtual bool IsAnimationProcess => IsAttackProccess || isWaitAnimationProccess || isWaitTransitionProccess;
	public bool IsAttackProccess { get; protected set; }

	public bool IsIdleTransaction => IsCurrentNodeName("IdleToIdleAction") || IsCurrentNodeName("IdleActionToIdle");


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
	protected int deathHash;
	protected int deathTypeHash;

	protected Animator animator;
	protected IEntityModel entity;

	[Inject]
	private void Construct(
		Animator animator,
		IEntityModel entity)
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

		deathHash = Animator.StringToHash("Death");
		deathTypeHash = Animator.StringToHash("DeathType");

		entity.onDestinationChanged += OnDestinationChanged;
		entity.Controller.onReachedDestination += OnReachedDestination;
	}

	protected virtual void Update()
	{
		Vector3 velocity = entity.Controller.GetVelocity();

		Vector3 horizontalVelocity = VectorMath.RemoveDotVector(velocity, transform.up);
		Vector3 verticalVelocity = velocity - horizontalVelocity;

		IsIdle = !entity.IsHasTarget && velocity.magnitude == 0;

		animator.applyRootMotion = IsRootMotion = IsIdle && !IsIdleTransaction;

		animator.SetFloat(forwardSpeedHash, velocity.magnitude);
		animator.SetFloat(verticalSpeedHash, verticalVelocity.magnitude * VectorMath.GetDotProduct(verticalVelocity, transform.up));
		//animator.SetFloat("HorizontalSpeed", Mathf.Clamp(controller.CalculateAngleToDesination(), -90, 90) / 90);

		animator.SetBool(isIdleHash, IsIdle);
		animator.SetBool(isGroundedHash, entity.Controller.IsGrounded);


		if (animator.applyRootMotion == false)
		{
			entity.Navigation.NavMeshAgent.nextPosition = transform.root.position;
		}
	}


	public virtual void Hit(int type = 0)
	{
		animator.SetInteger(hitTypeHash, type);
		animator.SetTrigger(hitHash);
	}
	public virtual void Attack() { }

	public virtual void Death(int type = 0)
	{
		animator.SetInteger(deathTypeHash, type);
		animator.SetTrigger(deathHash);
	}



	protected bool IsCurrentNodeName(string name)
	{
		return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
	}

	protected bool IsCurrentAnimationName(string animation)
	{
		return animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == animation;
	}

	protected bool IsCurrentTransitionName(string transition)
	{
		return animator.GetAnimatorTransitionInfo(0).IsName(transition);
	}

	protected IEnumerator WaitWhileNode(string node, bool isNot = false)
	{
		if (entity is IBattlable battlable)
		{
			isWaitAnimationProccess = true;

			while (battlable.InBattle)
			{
				if (isNot)
				{
					if (!IsCurrentNodeName(node))
					{
						break;
					}
				}
				else
				{
					if (IsCurrentNodeName(node))
					{
						break;
					}
				}

				
				yield return null;
			}

			isWaitAnimationProccess = false;
		}
	}

	protected IEnumerator WaitWhileAnimation(string animation)
	{
		if(entity is IBattlable battlable)
		{
			isWaitAnimationProccess = true;

			while (battlable.InBattle)
			{
				if (IsCurrentAnimationName(animation))
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
				if (IsCurrentTransitionName(transition))
				{
					break;
				}

				yield return null;
			}

			isWaitTransitionProccess = false;
		}
	}

	protected void SetLayerWeightByName(string layer, float weight)
	{
		animator.SetLayerWeight(animator.GetLayerIndex(layer), weight);
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