using CMF;

using DG.Tweening;
using Game.Entities;
using Game.Systems.BattleSystem;

using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

public class HumanoidAnimatorControl : AnimatorControl
{
	public UnityAction onAttackLeftHand;
	public UnityAction onAttackRightHand;
	public UnityAction onAttackKick;

	protected int weaponTypeHash;
	protected int attackTypeHash;
	protected int isAimingHash;

	private IBattlable humanoid;
	
	protected override void Start()
	{
		humanoid = entity as IBattlable;

		weaponTypeHash = Animator.StringToHash("WeaponType");
		attackTypeHash = Animator.StringToHash("AttackType");
		isAimingHash = Animator.StringToHash("IsAiming");
		base.Start();
	}

	protected override void Update()
	{
		base.Update();

		animator.SetBool(isBattleModeHash, humanoid.InBattle);

		if (humanoid.InBattle)
		{
			humanoid.Controller.IsWaitAnimation = IsAnimationProcess;
		}
	}

	public virtual void Attack(int weaponType = 0, int attackType = 0)
	{
		IsAttackProccess = true;

		animator.SetBool(isAimingHash, true);
		animator.SetInteger(weaponTypeHash, 2/*weaponType*/);
		animator.SetInteger(attackTypeHash, 0/*attackType*/);
		StartCoroutine(AttackProccess());
	}

	private IEnumerator AttackProccess()
	{
		yield return WaitWhileAnimation("Armature|IdleAction");

		animator.SetTrigger(attackHash);
		
		//while (true)
		//{
		//	string animationName = animator.GetCurrentAnimatorClipInfo(0)?[0].clip.name;

		//	transform.rotation = animator.rootRotation;

		//	if (animationName == "IdleFightToActionIdle")
		//	{
		//		break;
		//	}

		//	yield return null;
		//}

		transform.DOMove(transform.root.position, 0.25f);

		yield return WaitWhileAnimation("Armature|IdleAction");

		IsAttackProccess = false;
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