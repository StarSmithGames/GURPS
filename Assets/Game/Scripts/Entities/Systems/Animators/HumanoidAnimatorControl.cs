using DG.Tweening;

using ExitGames.Client.Photon.StructWrapping;

using Game.Entities;
using Game.Systems.BattleSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

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

	protected override void Start()
	{
		humanoid = entity as IBattlable;

		isAimingHash = Animator.StringToHash("IsAiming");

		base.Start();

		humanoid.onBattleChanged += OnBattleChanged;
		//(humanoid.Sheet as CharacterSheet).Equipment.WeaponCurrent.onEquipWeaponChanged += OnEquipWeaponChanged;
		OnEquipWeaponChanged();
	}

	protected override void Update()
	{
		base.Update();

		animator.SetBool(isBattleModeHash, humanoid.InBattle);

		if (humanoid.InBattle)
		{
			//humanoid.Controller.IsWaitAnimation = IsAnimationProcess;
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
			//(humanoid.Sheet as CharacterSheet).Equipment.WeaponCurrent.onEquipWeaponChanged -= OnEquipWeaponChanged;
		}
	}

	private void OnEquipWeaponChanged()
	{
		//CharacterSheet sheet = humanoid.Sheet as CharacterSheet;

		//Hands hands = sheet.Equipment.WeaponCurrent.Hands;

		//var weaponMain = sheet.Equipment.WeaponCurrent.Main.Item?.GetItemData<WeaponItemData>();

		//var lastBehabior = currentWeaponBehavior;

		//if (hands == Hands.Main || hands == Hands.Spare || (hands == Hands.Both && weaponMain is MeleeItemData melee && melee.melleType == MelleType.OneHanded))
		//{
		//	currentWeaponBehavior = new OneHandedBehavior(humanoid);
		//}
		//else if (hands == Hands.Both)
		//{
		//	if (weaponMain is MeleeItemData)
		//	{
		//		currentWeaponBehavior = new TwoHandedBehavior(humanoid);
		//	}
		//	else if (weaponMain is RangedItemData)
		//	{
		//		currentWeaponBehavior = new RangedBehavior(humanoid);
		//	}
		//}
		//else
		//{
		//	currentWeaponBehavior = new UnArmedBehavior(humanoid);
		//}


		//if (lastBehabior != null && lastBehabior != currentWeaponBehavior)
		//{
		//	lastBehabior.Dispose();
		//}

		//if (humanoid.InBattle)
		//{
		//	currentWeaponBehavior.UpdatePose();
		//}
	}

	private void OnBattleStateChanged()
	{
		if (humanoid.InBattle)
		{
			switch (humanoid.CurrentBattle.CurrentState)
			{
				case BattleState.PreBattle:
				{
					if(currentWeaponBehavior is DrawableWeapon drawable)
					{
						drawable.DrawWeapon();
					}
					break;
				}
				case BattleState.EndBattle:
				{
					if (currentWeaponBehavior is DrawableWeapon drawable)
					{
						drawable.SheathWeapon();
					}
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
		protected int weaponTypeHash;
		protected int attackTypeHash;

		protected IEquipment equipment;
		protected Animator animator;
		protected HumanoidAnimatorControl control;
		protected CharacterOutfit outfit;

		protected IBattlable owner;

		public WeaponBehavior(IBattlable owner)
		{
			this.owner = owner;
			//equipment = (owner.Sheet as CharacterSheet).Equipment;
			//outfit = (owner as CharacterModel).Outfit;
			//control = owner.AnimatorControl as HumanoidAnimatorControl;
			animator = control.animator;

			weaponTypeHash = Animator.StringToHash("WeaponType");
			attackTypeHash = Animator.StringToHash("AttackType");
		}

		public virtual void Dispose() { }

		public virtual void UpdatePose() { }

		public abstract void Attack();
	}

	public abstract class DrawableWeapon : WeaponBehavior
	{
		protected bool useAnimationDraw = true;
		protected bool useAnimationSheath = true;

		protected bool isAnimated = false;

		protected int drawTypeHash;//right - 0 left - 1 both - 2
		protected int drawWeaponHash;
		protected int sheathWeaponHash;

		protected DrawableWeapon(IBattlable owner) : base(owner)
		{
			drawTypeHash = Animator.StringToHash("DrawType");
			drawWeaponHash = Animator.StringToHash("DrawWeapon");
			sheathWeaponHash = Animator.StringToHash("SheathWeapon");

			control.onDrawWeapon += OnWeaponDrawed;
			control.onSheathWeapon += OnWeaponSheathed;

			outfit.Slots.Clear();
		}

		public override void UpdatePose()
		{
			isAnimated = false;
			
			if (owner.InBattle)
			{
				OnWeaponDrawed();
			}
			else
			{
				OnWeaponSheathed();
			}
		}

		public virtual void DrawWeapon() { }
		public virtual void SheathWeapon() { }

		protected virtual void DrawWeaponLeftHand()
		{
			isAnimated = true;

			control.SetLayerWeightByName(control.leftArmLayer, 0.7f);
			control.SetLayerWeightByName(control.rightArmLayer, 0f);

			animator.SetInteger(drawTypeHash, 1);//left hand
			animator.SetTrigger(drawWeaponHash);
		}
		protected virtual void SheathWeaponLeftHand()
		{
			isAnimated = true;
			
			control.SetLayerWeightByName(control.leftArmLayer, 0.7f);
			control.SetLayerWeightByName(control.rightArmLayer, 0f);

			animator.SetInteger(drawTypeHash, 1);//left hand
			animator.SetTrigger(sheathWeaponHash);
		}

		protected virtual void DrawWeaponRightHand()
		{
			isAnimated = true;

			control.SetLayerWeightByName(control.leftArmLayer, 0f);
			control.SetLayerWeightByName(control.rightArmLayer, 0.7f);

			animator.SetInteger(drawTypeHash, 0);//right hand
			animator.SetTrigger(drawWeaponHash);
		}
		protected virtual void SheathWeaponRightHand()
		{
			isAnimated = true;

			control.SetLayerWeightByName(control.leftArmLayer, 0f);
			control.SetLayerWeightByName(control.rightArmLayer, 0.7f);

			animator.SetInteger(drawTypeHash, 0);//right hand
			animator.SetTrigger(sheathWeaponHash);
		}

		protected virtual void DrawWeaponBothHand()
		{
			isAnimated = true;

			control.SetLayerWeightByName(control.leftArmLayer, 0.7f);
			control.SetLayerWeightByName(control.rightArmLayer, 0.7f);

			animator.SetInteger(drawTypeHash, 2);//both hands
			animator.SetTrigger(drawWeaponHash);
		}
		protected virtual void SheathWeaponBothHand()
		{
			isAnimated = true;

			control.SetLayerWeightByName(control.leftArmLayer, 0.7f);
			control.SetLayerWeightByName(control.rightArmLayer, 0.7f);

			animator.SetInteger(drawTypeHash, 2);//both hands
			animator.SetTrigger(sheathWeaponHash);
		}

		protected void AnimateLeftHand()
		{
			control.SetLayerWeightByName(control.leftArmLayer, owner.InBattle ? 0f : 0.7f);
			control.SetLayerWeightByName(control.leftHandLayer, 1f);
			control.SetLayerWeightByName(control.rightArmLayer, 0);
			control.SetLayerWeightByName(control.rightHandLayer, 0);
		}
		protected void AnimateRightHand()
		{
			control.SetLayerWeightByName(control.leftArmLayer, 0);
			control.SetLayerWeightByName(control.leftHandLayer, 0);
			control.SetLayerWeightByName(control.rightArmLayer, owner.InBattle ? 0f : 0.7f);
			control.SetLayerWeightByName(control.rightHandLayer, 1f);
		}
		protected void AnimateBothHands()
		{
			control.SetLayerWeightByName(control.leftArmLayer, owner.InBattle ? 0f : 0.7f);
			control.SetLayerWeightByName(control.leftHandLayer, 1f);
			control.SetLayerWeightByName(control.rightArmLayer, owner.InBattle ? 0f : 0.7f);
			control.SetLayerWeightByName(control.rightHandLayer, 1f);
		}

		public virtual void OnWeaponDrawed() { }
		public virtual void OnWeaponSheathed()
		{
			control.SetLayerWeightByName(control.rightArmLayer, 0f);
			control.SetLayerWeightByName(control.rightHandLayer, 0f);
			control.SetLayerWeightByName(control.leftArmLayer, 0f);
			control.SetLayerWeightByName(control.leftHandLayer, 0f);
		}
	}


	public class UnArmedBehavior : WeaponBehavior
	{
		public UnArmedBehavior(IBattlable owner) : base(owner)
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
			animator.SetInteger(weaponTypeHash, 0);
			animator.SetInteger(attackTypeHash, Random.Range(0, 3));
			animator.SetTrigger(control.attackHash);
		}
	}

	public class OneHandedBehavior : DrawableWeapon
	{
		private bool IsBoth => IsLeft && IsRight;
		private bool IsLeft => left != null;
		private bool IsRight => right != null;

		private ItemModel modelLeft;
		private ItemModel modelRight;

		private WeaponItemData right;
		private WeaponItemData left;

		public OneHandedBehavior(IBattlable owner) : base(owner)
		{
			useAnimationDraw = false;

			right = equipment.WeaponCurrent.Main.Item?.GetItemData<WeaponItemData>();
			left = equipment.WeaponCurrent.Spare.Item?.GetItemData<WeaponItemData>();

			UpdatePose();
		}

		public override void Dispose()
		{
			control.onDrawWeapon -= OnWeaponDrawed;
			control.onSheathWeapon -= OnWeaponSheathed;
		}


		public override void Attack()
		{
			animator.SetInteger(weaponTypeHash, 1);
			animator.SetInteger(attackTypeHash, Random.Range(0, 5));
			animator.SetTrigger(control.attackHash);
		}
		public override void DrawWeapon()
		{
			DrawWeaponBothHand();
		}
		public override void SheathWeapon()
		{
			if (IsBoth)
			{
				SheathWeaponBothHand();
			}

		}

		public override void OnWeaponDrawed()
		{
			if (IsBoth)
				AnimateBothHands();
			else if (IsLeft)
				AnimateLeftHand();
			else if (IsRight)
				AnimateRightHand();


			if (modelLeft == null && left != null)
			{
				modelLeft = GameObject.Instantiate(left.prefab);

				isAnimated = false;
			}

			if (modelRight == null && right != null)
			{
				modelRight = GameObject.Instantiate(right.prefab);

				isAnimated = false;
			}


			if (modelLeft != null)
			{
				outfit.Slots.leftHand.Set(modelLeft.transform, left.leftHandTransfrom, useAnimationDraw ? isAnimated : false);
			}
			if (modelRight != null)
			{
				outfit.Slots.rightHand.Set(modelRight.transform, right.rightHandTransfrom, useAnimationDraw ? isAnimated : false);
			}
		}
		public override void OnWeaponSheathed()
		{
			base.OnWeaponSheathed();

			if (modelLeft == null && left != null)
			{
				modelLeft = GameObject.Instantiate(left.prefab);

				isAnimated = false;
			}

			if (modelRight == null && right != null)
			{
				modelRight = GameObject.Instantiate(right.prefab);

				isAnimated = false;
			}


			if (modelLeft != null)
			{
				outfit.Slots.leftSheath.Set(modelLeft.transform, left.sheathForLeftHandTransfrom, useAnimationSheath ? isAnimated : false);
			}
			if (modelRight != null)
			{
				outfit.Slots.rightSheath.Set(modelRight.transform, right.sheathForRightHandTransfrom, useAnimationSheath ? isAnimated : false);
			}
		}
	}

	public class TwoHandedBehavior : DrawableWeapon
	{
		private ItemModel model;

		private WeaponItemData data;

		public TwoHandedBehavior(IBattlable owner) : base(owner)
		{
			data = equipment.WeaponCurrent.Main.Item.GetItemData<WeaponItemData>();

			UpdatePose();
		}

		public override void Dispose()
		{
			control.onDrawWeapon -= OnWeaponDrawed;
			control.onSheathWeapon -= OnWeaponSheathed;
		}


		public override void Attack()
		{
			animator.SetInteger(weaponTypeHash, 1);
			animator.SetInteger(attackTypeHash, 1);
			animator.SetTrigger(control.attackHash);
		}
		public override void DrawWeapon()
		{
			DrawWeaponRightHand();
		}
		public override void SheathWeapon()
		{
			SheathWeaponRightHand();
		}

		public override void OnWeaponDrawed()
		{
			AnimateRightHand();

			if (model == null)
			{
				model = GameObject.Instantiate(data.prefab);

				isAnimated = false;
			}

			outfit.Slots.rightHand.Set(model.transform, data.rightHandTransfrom, useAnimationDraw ? isAnimated : false);
		}
		public override void OnWeaponSheathed()
		{
			base.OnWeaponSheathed();

			if (model == null)
			{
				model = GameObject.Instantiate(data.prefab);

				isAnimated = false;
			}

			outfit.Slots.backSheath.Set(model.transform, data.sheathForRightHandTransfrom, useAnimationSheath ? isAnimated : false);
		}
	}

	public class RangedBehavior : DrawableWeapon
	{
		private ItemModel model;

		private WeaponItemData data;

		public RangedBehavior(IBattlable owner) : base(owner)
		{
			data = equipment.WeaponCurrent.Main.Item.GetItemData<WeaponItemData>();

			outfit.Slots.Clear();

			UpdatePose();
		}

		public override void Dispose()
		{
			control.onDrawWeapon -= OnWeaponDrawed;
			control.onSheathWeapon -= OnWeaponSheathed;
		}

		public override void Attack()
		{
			animator.SetInteger(weaponTypeHash, 3);
			animator.SetInteger(attackTypeHash, 0);
			animator.SetTrigger(control.attackHash);
		}
		public override void DrawWeapon()
		{
			DrawWeaponLeftHand();
		}
		public override void SheathWeapon()
		{
			SheathWeaponLeftHand();
		}


		public override void OnWeaponDrawed()
		{
			AnimateLeftHand();

			if (model == null)
			{
				model = GameObject.Instantiate(data.prefab);

				isAnimated = false;
			}

			outfit.Slots.leftHand.Set(model.transform, data.leftHandTransfrom, useAnimationDraw ? isAnimated : false);
		}

		public override void OnWeaponSheathed()
		{
			base.OnWeaponSheathed();

			if (model == null)
			{
				model = GameObject.Instantiate(data.prefab);

				isAnimated = false;
			}

			outfit.Slots.backSheath.Set(model.transform, data.sheathForLeftHandTransfrom, useAnimationSheath ? isAnimated : false);
		}
	}
}
#endregion