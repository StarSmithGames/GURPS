using CMF;

using Game.Entities.Models;
using Game.Systems.BattleSystem;

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Systems.AnimatorController
{
	public partial class AnimatorController : MonoBehaviour
	{
		public delegate void AttackTriggerEvent();

		public event AttackTriggerEvent onAttack;

		public bool IsRootMotion { get; private set; }
		public bool IsIdle { get; private set; }

		public virtual bool IsAnimationProcess => IsAttackProccess || isWaitAnimationProccess || isWaitTransitionProccess;
		public bool IsAttackProccess { get; protected set; }


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

		protected SignalBus signalBus;
		protected Animator animator;
		protected ICharacterModel characterModel;

		[Inject]
		private void Construct(SignalBus signalBus, Animator animator, ICharacterModel characterModel)
		{
			this.signalBus = signalBus;
			this.animator = animator;
			this.characterModel = characterModel;
		}

		public virtual void Initialize()
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

			characterModel.Controller.onReachedDestination += OnReachedDestination;
		}

		protected virtual void OnDestroy()
		{
			if (characterModel != null)
			{
				characterModel.Controller.onReachedDestination -= OnReachedDestination;
			}
		}

		protected virtual void Update()
		{
			//Movement
			Vector3 velocity = characterModel.Controller.GetVelocity();

			Vector3 horizontalVelocity = VectorMath.RemoveDotVector(velocity, transform.up);
			Vector3 verticalVelocity = velocity - horizontalVelocity;

			IsIdle = !characterModel.IsHasTarget && velocity.magnitude == 0;

			animator.applyRootMotion = IsRootMotion = IsIdle && !IsIdleTransaction;

			animator.SetFloat(forwardSpeedHash, velocity.magnitude);
			animator.SetFloat(verticalSpeedHash, verticalVelocity.magnitude * VectorMath.GetDotProduct(verticalVelocity, transform.up));
			//animator.SetFloat("HorizontalSpeed", Mathf.Clamp(controller.CalculateAngleToDesination(), -90, 90) / 90);

			animator.SetBool(isIdleHash, IsIdle);
			animator.SetBool(isGroundedHash, characterModel.Controller.IsGrounded);


			if (animator.applyRootMotion == false)
			{
				characterModel.Navigation.NavMeshAgent.nextPosition = transform.root.position;
			}
		}

		public void EnableBattleMode(bool trigger)
		{
			animator.SetBool(isBattleModeHash, trigger);
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

		protected IEnumerator WaitWhileAnimation(string animation)
		{
			if (characterModel is IBattlable battlable)
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
			if (characterModel is IBattlable battlable)
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


		private void OnReachedDestination()
		{
			if (characterModel is IBattlable battlable)
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
			Debug.LogError("AttackEvent");
			onAttack?.Invoke();
		}
		#endregion
	}

	public partial class AnimatorController
	{
		public bool IsIdleTransaction => IsCurrentNodeName("IdleToIdleAction") || IsCurrentNodeName("IdleActionToIdle");

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
	}
}