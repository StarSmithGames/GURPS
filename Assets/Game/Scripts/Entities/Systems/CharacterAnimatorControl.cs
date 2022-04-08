using CMF;

using DG.Tweening;
using Game.Entities;

using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

public class CharacterAnimatorControl : AnimatorControl
{
	public UnityAction onAttackLeftHand;
	public UnityAction onAttackRightHand;
	public UnityAction onAttackKick;

	protected int weaponTypeHash;
	protected int attackTypeHash;

	private Character character;
	private CharacterOutfit outfit;
	
	[Inject]
	private void Construct(CharacterOutfit outfit)
	{
		this.outfit = outfit;

		character = entity as Character;
	}

	protected override void Start()
	{
		weaponTypeHash = Animator.StringToHash("WeaponType");
		attackTypeHash = Animator.StringToHash("AttackType");
		base.Start();
	}

	protected override void Update()
	{
		base.Update();

		animator.SetBool(isBattleModeHash, character.InBattle);

		if (character.InBattle)
		{
			character.Controller.IsWaitAnimation = IsAnimationProcess;
		}
	}

	public virtual void Attack(int weaponType = 0, int attackType = 0)
	{
		animator.SetInteger(weaponTypeHash, weaponType);
		animator.SetInteger(attackTypeHash, attackType);
		StartCoroutine(AttackProccess());
	}

	private IEnumerator AttackProccess()
	{
		isAttackProccess = true;

		yield return WaitWhileAnimation("Armature|IdleAction");

		animator.applyRootMotion = true;

		animator.SetTrigger(attackHash);
		
		while (true)
		{
			string animationName = animator.GetCurrentAnimatorClipInfo(0)?[0].clip.name;

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

	#region AnimationEvents
	private void AttackLeftHand()
	{
		onAttackLeftHand?.Invoke();
	}
	private void AttackRightHand()
	{
		onAttackRightHand?.Invoke();
	}
	private void AttackKick()
	{
		onAttackKick?.Invoke();
	}
	#endregion
}