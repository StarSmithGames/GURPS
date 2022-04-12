using DG.Tweening;
using Game.Systems.BattleSystem;

using System.Collections;

using UnityEngine;
using UnityEngine.Events;

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
		animator.SetInteger(weaponTypeHash, weaponType);
		animator.SetInteger(attackTypeHash, attackType);

		StartCoroutine(AttackProcess());
	}

	private IEnumerator AttackProcess()
	{
		yield return WaitWhileNode("IdleAction");
		animator.SetTrigger(attackHash);
		yield return WaitWhileNode("IdleAction", true);
		yield return WaitWhileNode("IdleAction");
		transform.DOMove(transform.root.position, 0.25f);
		IsAttackProccess = false;
	}

	//private IEnumerator UnArmedAttackProcess()
	//{
	//	yield return WaitWhileAnimation("Armature|IdleAction");

	//	animator.SetTrigger(attackHash);

	//	float startTime = Time.time;

	//	while (true)
	//	{
	//		transform.rotation = animator.rootRotation;

	//		if (IsCurrentAnimationName("Armature|IdleFightToIdleAction"))
	//		{
	//			break;
	//		}

	//		if(Time.time - startTime > 5f)
	//		{
	//			Debug.LogError("ERROR ATTACK PROCESS STUCK");
	//			IsAttackProccess = false;
	//			transform.DOMove(transform.root.position, 0.25f);
	//			yield break;
	//		}

	//		yield return null;
	//	}
	//	transform.DOMove(transform.root.position, 0.25f);
	//	yield return WaitWhileAnimation("Armature|IdleAction");

	//	IsAttackProccess = false;
	//}

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

	private void DrawOneHandedWeapon()
	{
		Debug.LogError("DrawOneHandedWeapon");
	}
	private void DrawTwoHandedWeapon(int type = 0)
	{
		Debug.LogError("DrawTwoHandedWeapon");
		if(type == 0)//bow
		{

		}
	}
	private void DrawArrow()
	{

	}
	#endregion
}