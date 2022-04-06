using DG.Tweening;
using Game.Entities;

using System.Collections;

using UnityEngine;

using Zenject;

public class CharacterAnimatorControl : AnimatorControl
{
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

		animator.SetInteger(weaponTypeHash, (int)outfit.CurrentWeaponType);

		animator.SetBool(isBattleModeHash, character.InBattle);

		if (character.InBattle)
		{
			character.Controller.IsWaitAnimation = isAttackProccess ||
				isWaitAnimationProccess ||
				isWaitTransitionProccess;
		}
	}

	public override void Attack(int type = -1)
	{
		animator.SetInteger(attackTypeHash, type == -1 ? 0 : type);
		StartCoroutine(AttackProccess());
	}

	private IEnumerator AttackProccess()
	{
		Vector3 lastForward = transform.forward;

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
		transform.DORotate(lastForward, 0.25f);

		yield return WaitWhileAnimation("Armature|IdleAction");

		animator.applyRootMotion = false;
		isAttackProccess = false;
	}
}