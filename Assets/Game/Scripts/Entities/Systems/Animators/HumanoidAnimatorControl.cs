using DG.Tweening;

using Game.Entities;
using Game.Systems.BattleSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

using static HumanoidAnimatorControl;

public partial class HumanoidAnimatorControl : AnimatorControl
{
	public UnityAction onAttackLeftHand;
	public UnityAction onAttackRightHand;
	public UnityAction onAttackKick;

	public UnityAction onDrawWeapon;
	public UnityAction onSheathWeapon;


	protected int isAimingHash;

	private string leftArmLayer = "LeftArm";
	private string rightArmLayer = "RightArm";
	private string bothHandsLayer = "BothHands";
	private string leftHandLayer = "LeftHand";
	private string rightHandLayer = "RightHand";

	private IBattlable humanoid;
	private WeaponBehavior weaponBehavior;

	private CharacterOutfit outfit;

	protected override void Start()
	{
		humanoid = entity as IBattlable;

		outfit = GetComponent<CharacterOutfit>();//stub

		isAimingHash = Animator.StringToHash("IsAiming");

		base.Start();

		humanoid.onBattleChanged += OnBattleChanged;
		(humanoid.Sheet as CharacterSheet).Equipment.WeaponCurrent.onEquipWeaponChanged += OnEquipWeaponChanged;
		OnEquipWeaponChanged();
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

	public override void Attack()
	{
		StartCoroutine(AttackProcess());
	}

	private IEnumerator AttackProcess()
	{
		IsAttackProccess = true;
		yield return WaitWhileNode("IdleAction");
		weaponBehavior.Attack();
		yield return WaitWhileNode("IdleAction", true);
		yield return WaitWhileNode("IdleAction");
		transform.DOMove(transform.root.position, 0.25f);
		IsAttackProccess = false;
	}

	private void OnEquipWeaponChanged()
	{
		CharacterSheet sheet = humanoid.Sheet as CharacterSheet;

		var weaponMain = sheet.Equipment.WeaponCurrent.Main.Item?.GetItemData<WeaponItemData>();
		var weaponSpare = sheet.Equipment.WeaponCurrent.Spare.Item?.GetItemData<WeaponItemData>();

		switch (sheet.Equipment.WeaponCurrent.Hands)
		{
			case Hands.None:
			{
				weaponBehavior = new UnArmedBehavior(this, outfit);
				break;
			}
			case Hands.Main:
			case Hands.Spare:
			{
				weaponBehavior = new OneHandedBehavior(this, outfit, weaponMain, weaponSpare, !humanoid.InBattle);
				break;
			}
			case Hands.Both:
			{
				if(weaponMain is MeleeItemData melee)
				{
					switch (melee.melleType)
					{
						case MelleType.OneHanded:
						{
							weaponBehavior = new OneHandedBehavior(this, outfit, weaponMain, weaponSpare, !humanoid.InBattle);
							break;
						}
						case MelleType.TwoHanded:
						{
							weaponBehavior = new TwoHandedBehavior(this, outfit, weaponMain, !humanoid.InBattle);
							break;
						}
					}
				}
				else if (weaponMain is RangedItemData)
				{
					weaponBehavior = new RangedBehavior(this, outfit, weaponMain, !humanoid.InBattle);
				}
				break;
			}
		}

		if (humanoid.InBattle)
		{
			weaponBehavior.UpdatePose();
		}
	}

	private void OnBattleStateChanged()
	{
		if (humanoid.InBattle)
		{
			switch (humanoid.CurrentBattle.CurrentState)
			{
				case BattleState.PreBattle:
				{
					weaponBehavior.DrawWeapon();
					break;
				}
				case BattleState.EndBattle:
				{
					weaponBehavior.SheathWeapon();
					break;
				}
			}
		}
	}

	public abstract class WeaponBehavior
	{
		protected Animator Animator => control.animator;
		protected HumanoidAnimatorControl control;
		protected CharacterOutfit outfit;

		protected int drawTypeHash;//right - 0 left - 1 both - 2
		protected int weaponTypeHash;
		protected int attackTypeHash;

		protected int drawWeaponHash;
		protected int sheathWeaponHash;

		public WeaponBehavior(HumanoidAnimatorControl control, CharacterOutfit outfit)
		{
			this.control = control;
			this.outfit = outfit;

			drawTypeHash = Animator.StringToHash("DrawType");
			weaponTypeHash = Animator.StringToHash("WeaponType");
			attackTypeHash = Animator.StringToHash("AttackType");

			drawWeaponHash = Animator.StringToHash("DrawWeapon");
			sheathWeaponHash = Animator.StringToHash("SheathWeapon");
		}

		public virtual void UpdatePose() { }

		public virtual void DrawWeapon() { }

		public abstract void Attack();

		public virtual void SheathWeapon() { }
	}

	public class UnArmedBehavior : WeaponBehavior
	{
		public UnArmedBehavior(HumanoidAnimatorControl control, CharacterOutfit outfit) : base(control, outfit)
		{
			UpdatePose();
		}

		public override void UpdatePose()
		{
			control.SetLayerWeightByName(control.leftArmLayer, 0f);
			control.SetLayerWeightByName(control.leftHandLayer, 0f);
			control.SetLayerWeightByName(control.rightHandLayer, 0f);
		
			outfit.Slots.Clear();
		}

		public override void Attack()
		{
			Animator.SetInteger(weaponTypeHash, 0);
			Animator.SetInteger(attackTypeHash, Random.Range(0, 3));
			Animator.SetTrigger(control.attackHash);
		}
	}

	public class OneHandedBehavior : WeaponBehavior
	{
		private WeaponItemData right;
		private WeaponItemData left;
		private bool isInSheath;

		public OneHandedBehavior(HumanoidAnimatorControl control, CharacterOutfit outfit, WeaponItemData right, WeaponItemData left, bool isInSheath) : base(control, outfit)
		{
			this.right = right;
			this.left = left;
			this.isInSheath = isInSheath;

			UpdatePose();
		}

		public override void UpdatePose()
		{
			if (isInSheath)
			{
				if (right != null)
				{
					outfit.Slots.rightSheath.Replace(right.prefab, right.sheathForRightHandTransfrom);
				}

				if (left != null)
				{
					outfit.Slots.leftSheath.Replace(left.prefab, left.sheathForLeftHandTransfrom);
				}
			}
			else
			{
				control.SetLayerWeightByName(control.leftArmLayer, 0f);
				control.SetLayerWeightByName(control.leftHandLayer, left != null ? 1f : 0f);
				control.SetLayerWeightByName(control.rightHandLayer, right != null ? 1f : 0f);

				if (right != null)
				{
					outfit.Slots.rightHand.Replace(right.prefab, right.rightHandTransfrom);
				}

				if (left != null)
				{
					outfit.Slots.leftHand.Replace(left.prefab, left.leftHandTransfrom);
				}
			}
		}

		public override void DrawWeapon()
		{
			UpdatePose();
			if (right != null)
			{
				Animator.SetInteger(drawTypeHash, 0);
			}
			if (left != null)
			{
				Animator.SetInteger(drawTypeHash, 1);
			}
			Animator.SetTrigger(drawWeaponHash);
		}

		public override void Attack()
		{
			Animator.SetInteger(weaponTypeHash, 1);
			Animator.SetInteger(attackTypeHash, 0);
			Animator.SetTrigger(control.attackHash);
		}

		public override void SheathWeapon()
		{
			Animator.SetInteger(drawTypeHash, right != null && left != null ? 2 : (right != null ? 0 : (left != null ? 1 : 0)));

			Sequence sequence = DOTween.Sequence();

			sequence
				.AppendCallback(() => Animator.SetTrigger(sheathWeaponHash))
				.AppendInterval(1f)
				.AppendCallback(() =>
				{
					if (right != null)
					{
						control.SetLayerWeightByName(control.rightArmLayer, 0f);
						control.SetLayerWeightByName(control.rightHandLayer, 0f);
					}
					if (left != null)
					{
						control.SetLayerWeightByName(control.leftArmLayer, 0f);
						control.SetLayerWeightByName(control.leftHandLayer, 0f);
					}
				});
		}
	}

	public class TwoHandedBehavior : WeaponBehavior
	{
		private WeaponItemData data;
		private bool isInSheath;

		public TwoHandedBehavior(HumanoidAnimatorControl control, CharacterOutfit outfit, WeaponItemData data, bool isInSheath) : base(control, outfit)
		{
			this.data = data;
			this.isInSheath = isInSheath;

			control.onDrawWeapon += OnWeaponDrawed;
			control.onSheathWeapon += OnWeaponSheathed;

			UpdatePose();
		}

		public override void UpdatePose()
		{
			if (isInSheath)
			{
				outfit.Slots.Clear();
				outfit.Slots.backSheath.Replace(data.prefab, data.sheathForRightHandTransfrom);
			}
			else
			{
				OnWeaponDrawed();
			}
		}

		public override void DrawWeapon()
		{
			control.SetLayerWeightByName(control.rightArmLayer, 0.7f);
			Animator.SetInteger(drawTypeHash, 0);//right hand
			Animator.SetTrigger(drawWeaponHash);
		}

		public override void Attack()
		{
			Animator.SetInteger(weaponTypeHash, 2);
			Animator.SetInteger(attackTypeHash, 0);
			Animator.SetTrigger(control.attackHash);
		}

		public override void SheathWeapon()
		{
			Animator.SetInteger(drawTypeHash, 0);//right hand
			Animator.SetTrigger(sheathWeaponHash);
		}


		private void OnWeaponDrawed()
		{
			control.SetLayerWeightByName(control.rightArmLayer, 0.7f);
			control.SetLayerWeightByName(control.rightHandLayer, 1f);

			outfit.Slots.Clear();
			outfit.Slots.rightHand.Replace(data.prefab, data.rightHandTransfrom);
		}

		private void OnWeaponSheathed()
		{
			control.SetLayerWeightByName(control.rightArmLayer, 0f);
			control.SetLayerWeightByName(control.rightHandLayer, 0f);

			outfit.Slots.Clear();
			outfit.Slots.backSheath.Replace(data.prefab, data.sheathForRightHandTransfrom);
		}
	}

	public class RangedBehavior : WeaponBehavior
	{
		private WeaponItemData data;
		private bool isInSheath;

		public RangedBehavior(HumanoidAnimatorControl control, CharacterOutfit outfit, WeaponItemData data, bool isInSheath) : base(control, outfit)
		{
			this.data = data;
			this.isInSheath = isInSheath;

			control.onDrawWeapon += OnWeaponDrawed;
			control.onSheathWeapon += OnWeaponSheathed;

			UpdatePose();
		}

		public override void UpdatePose()
		{
			if (isInSheath)
			{
				outfit.Slots.Clear();
				outfit.Slots.backSheath.Replace(data.prefab, data.sheathForLeftHandTransfrom);
			}
			else
			{
				OnWeaponDrawed();
			}
		}

		public override void DrawWeapon()
		{
			control.SetLayerWeightByName(control.leftArmLayer, 0.7f);

			Animator.SetInteger(drawTypeHash, 1);//left hand
			Animator.SetTrigger(drawWeaponHash);
		}

		public override void Attack()
		{
			Animator.SetInteger(weaponTypeHash, 3);
			Animator.SetInteger(attackTypeHash, 0);
			Animator.SetTrigger(control.attackHash);
		}

		public override void SheathWeapon()
		{
			Animator.SetInteger(drawTypeHash, 1);//left hand
			Animator.SetTrigger(sheathWeaponHash);
		}


		private void OnWeaponDrawed()
		{
			control.SetLayerWeightByName(control.leftArmLayer, 0.7f);
			control.SetLayerWeightByName(control.leftHandLayer, 1f);
			
			outfit.Slots.Clear();
			outfit.Slots.leftHand.Replace(data.prefab, data.leftHandTransfrom);
		}

		private void OnWeaponSheathed()
		{
			control.SetLayerWeightByName(control.leftArmLayer, 0f);
			control.SetLayerWeightByName(control.leftHandLayer, 0f);

			outfit.Slots.Clear();
			outfit.Slots.backSheath.Replace(data.prefab, data.sheathForLeftHandTransfrom);
		}
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

	private void DrawWeapon()
	{
		onDrawWeapon?.Invoke();
	}
	private void SheathWeapon()
	{
		onSheathWeapon?.Invoke();
	}


	private void DrawArrow()
	{

	}
	#endregion
}

partial class HumanoidAnimatorControl
{
	private Battle currentBattle;

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if (humanoid != null)
		{
			humanoid.onBattleChanged -= OnBattleChanged;
			(humanoid.Sheet as CharacterSheet).Equipment.WeaponCurrent.onEquipWeaponChanged -= OnEquipWeaponChanged;
		}
	}

	private void OnBattleChanged()
	{
		if (humanoid.CurrentBattle != null)
		{
			if (currentBattle != null)
			{
				currentBattle.onBattleStateChanged -= OnBattleStateChanged;
			}

			currentBattle = humanoid.CurrentBattle;
			currentBattle.onBattleStateChanged += OnBattleStateChanged;
		}
		else
		{
			if (currentBattle != null)
			{
				currentBattle.onBattleStateChanged -= OnBattleStateChanged;
			}
		}
	}
}