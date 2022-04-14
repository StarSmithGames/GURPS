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
	private string leftHandLayer = "LeftHand";
	private string rightHandLayer = "RightHand";

	private IBattlable humanoid;
	private WeaponBehavior currentWeaponBehavior;

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
		currentWeaponBehavior.Attack();
		yield return WaitWhileNode("IdleAction", true);
		yield return WaitWhileNode("IdleAction");
		transform.DOMove(transform.root.position, 0.25f);
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

#region WeaponBehavior
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

	private void OnEquipWeaponChanged()
	{
		CharacterSheet sheet = humanoid.Sheet as CharacterSheet;

		Hands hands = sheet.Equipment.WeaponCurrent.Hands;

		var weaponMain = sheet.Equipment.WeaponCurrent.Main.Item?.GetItemData<WeaponItemData>();
		var weaponSpare = sheet.Equipment.WeaponCurrent.Spare.Item?.GetItemData<WeaponItemData>();

		bool isInSheath = !humanoid.InBattle;

		var lastBehabior = currentWeaponBehavior;

		if(hands == Hands.Main || hands == Hands.Spare || (hands == Hands.Both && weaponMain is MeleeItemData melee && melee.melleType == MelleType.OneHanded))
		{
			if(currentWeaponBehavior != null && currentWeaponBehavior is OneHandedBehavior behavior)
			{
				behavior.Reset(weaponMain, weaponSpare, isInSheath);
			}
			else
			{
				currentWeaponBehavior = new OneHandedBehavior(this, outfit, weaponMain, weaponSpare, isInSheath);
			}
		}
		else if (hands == Hands.Both)
		{
			if(weaponMain is MeleeItemData)
			{
				if (currentWeaponBehavior != null && currentWeaponBehavior is TwoHandedBehavior behavior)
				{
					behavior.Reset(weaponMain, isInSheath);
				}
				else
				{
					currentWeaponBehavior = new TwoHandedBehavior(this, outfit, weaponMain, isInSheath);
				}
			}
			else if(weaponMain is RangedItemData)
			{
				if (currentWeaponBehavior != null && currentWeaponBehavior is TwoHandedBehavior behavior)
				{
					behavior.Reset(weaponMain, isInSheath);
				}
				else
				{
					currentWeaponBehavior = new RangedBehavior(this, outfit, weaponMain, isInSheath);
				}
			}
		}
		else
		{
			currentWeaponBehavior = new UnArmedBehavior(this, outfit);
		}


		if (lastBehabior != null && lastBehabior != currentWeaponBehavior)
		{
			lastBehabior.Dispose();
		}

		if (humanoid.InBattle)
		{
			currentWeaponBehavior.UpdatePose();
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
					currentWeaponBehavior.DrawWeapon();
					break;
				}
				case BattleState.EndBattle:
				{
					currentWeaponBehavior.SheathWeapon();
					break;
				}
			}
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

		public virtual void Dispose() { }


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
			control.SetLayerWeightByName(control.rightArmLayer, 0f);
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
		private bool IsBoth => IsLeft && IsRight;
		private bool IsLeft => left != null;
		private bool IsRight => right != null;

		private WeaponItemData right;
		private WeaponItemData left;
		private bool isInSheath;

		public OneHandedBehavior(HumanoidAnimatorControl control, CharacterOutfit outfit, WeaponItemData right, WeaponItemData left, bool isInSheath) : base(control, outfit)
		{
			Reset(right, left, isInSheath);
		}

		public override void Dispose()
		{
			control.onDrawWeapon -= OnWeaponDrawed;
			control.onSheathWeapon -= OnWeaponSheathed;
		}

		public void Reset(WeaponItemData right, WeaponItemData left, bool isInSheath)
		{
			Dispose();

			this.right = right;
			this.left = left;
			this.isInSheath = isInSheath;

			control.onDrawWeapon += OnWeaponDrawed;
			control.onSheathWeapon += OnWeaponSheathed;

			UpdatePose();
		}

		public override void UpdatePose()
		{
			if (isInSheath)
			{
				OnWeaponSheathed();
			}
			else
			{
				OnWeaponDrawed();
			}
		}

		public override void DrawWeapon()
		{
			control.SetLayerWeightByName(control.leftArmLayer, IsLeft ? 0.7f : 0f);
			control.SetLayerWeightByName(control.rightArmLayer, IsRight ? 0.7f : 0f);

			Animator.SetInteger(drawTypeHash, IsBoth ? 2 : (IsRight ? 0 : 1));
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
			Animator.SetInteger(drawTypeHash, IsBoth ? 2 : (IsRight ? 0 : 1));
			Animator.SetTrigger(sheathWeaponHash);
		}


		private void OnWeaponDrawed()
		{
			control.SetLayerWeightByName(control.leftArmLayer, IsLeft ? 0.7f : 0f);
			control.SetLayerWeightByName(control.rightArmLayer, IsRight ? 0.7f : 0f);
			control.SetLayerWeightByName(control.leftHandLayer, IsLeft ? 1f : 0f);
			control.SetLayerWeightByName(control.rightHandLayer, IsRight ? 1f : 0f);

			outfit.Slots.Clear();

			if (IsRight)
			{
				outfit.Slots.rightHand.Replace(right.prefab, right.rightHandTransfrom);
			}

			if (IsLeft)
			{
				outfit.Slots.leftHand.Replace(left.prefab, left.leftHandTransfrom);
			}
		}

		private void OnWeaponSheathed()
		{
			control.SetLayerWeightByName(control.leftArmLayer, 0f);
			control.SetLayerWeightByName(control.rightArmLayer, 0f);
			control.SetLayerWeightByName(control.leftHandLayer, 0f);
			control.SetLayerWeightByName(control.rightHandLayer, 0f);

			outfit.Slots.Clear();

			if (IsRight)
			{
				outfit.Slots.rightSheath.Replace(right.prefab, right.sheathForRightHandTransfrom);
			}

			if (IsLeft)
			{
				outfit.Slots.leftSheath.Replace(left.prefab, left.sheathForLeftHandTransfrom);
			}
		}
	}

	public class TwoHandedBehavior : WeaponBehavior
	{
		private WeaponItemData data;
		private bool isInSheath;

		public TwoHandedBehavior(HumanoidAnimatorControl control, CharacterOutfit outfit, WeaponItemData data, bool isInSheath) : base(control, outfit)
		{
			Reset(data, isInSheath);
		}

		public override void Dispose()
		{
			control.onDrawWeapon -= OnWeaponDrawed;
			control.onSheathWeapon -= OnWeaponSheathed;
		}

		public void Reset(WeaponItemData data, bool isInSheath)
		{
			Dispose();

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
				OnWeaponSheathed();
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
			Reset(data, isInSheath);
		}

		public override void Dispose()
		{
			control.onDrawWeapon -= OnWeaponDrawed;
			control.onSheathWeapon -= OnWeaponSheathed;
		}

		public void Reset(WeaponItemData data, bool isInSheath)
		{
			Dispose();

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
				OnWeaponSheathed();
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
}
#endregion