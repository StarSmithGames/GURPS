using DG.Tweening;

using Game.Entities;
using Game.Entities.Models;
using Game.Systems.BattleSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using Zenject;

using Random = UnityEngine.Random;

namespace Game.Systems.AnimatorController
{
	public partial class HumanoidAnimatorController : AnimatorController
	{
		public event AttackTriggerEvent onAttackLeftHand;
		public event AttackTriggerEvent onAttackRightHand;
		public event AttackTriggerEvent onAttackKick;

		public event UnityAction onDrawWeapon;
		public event UnityAction onSheathWeapon;

		protected int isAimingHash;

		private string leftArmLayer = "LeftArm";
		private string rightArmLayer = "RightArm";
		private string leftHandLayer = "LeftHand";
		private string rightHandLayer = "RightHand";

		private string nodeIdleAction = "IdleAction";

		private ICharacterModel humanoid;
		private WeaponBehavior currentWeaponBehavior;

		public override void Initialize()
		{
			humanoid = entity as ICharacterModel;

			isAimingHash = Animator.StringToHash("IsAiming");

			base.Initialize();

			signalBus?.Subscribe<SignalJoinBattleLocal>(OnJoinedBattle);
			signalBus?.Subscribe<SignalLeaveBattleLocal>(OnLeavedBattle);

			(humanoid.Sheet as CharacterSheet).Equipment.WeaponCurrent.onEquipWeaponChanged += OnEquipWeaponChanged;
			OnEquipWeaponChanged();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			signalBus?.Unsubscribe<SignalJoinBattleLocal>(OnJoinedBattle);
			signalBus?.Unsubscribe<SignalLeaveBattleLocal>(OnLeavedBattle);

			if (humanoid != null)
			{
				(humanoid.Sheet as CharacterSheet).Equipment.WeaponCurrent.onEquipWeaponChanged -= OnEquipWeaponChanged;
			}
		}

		protected override void Update()
		{
			base.Update();


			if (humanoid.InBattle)
			{
				//humanoid.Controller.IsWaitAnimation = IsAnimationProcess;
			}
		}



		public override void Attack()
		{
			StartCoroutine(AttackProcess(currentWeaponBehavior.Attack));
		}

		public void AttackKick()
		{
			SetBehaviorToUnArmed();
			StartCoroutine(AttackProcess((currentWeaponBehavior as UnArmedBehavior).Kick));
		}

		//transform.DOMove(transform.root.position, 0.25f);

		private IEnumerator AttackProcess(UnityAction attack)
		{
			IsAttackProccess = true;

			yield return EnterIdleAction();
			yield return Attacking(attack);
			yield return ExitIdleAction();

			IsAttackProccess = false;
		}

		private IEnumerator Attacking(UnityAction attack)
		{
			attack?.Invoke();

			yield return new WaitUntil(() => !IsCurrentNodeName(nodeIdleAction));
			yield return new WaitUntil(() => IsCurrentNodeName(nodeIdleAction));
		}

		private IEnumerator EnterIdleAction()
		{
			if (!humanoid.InBattle)
			{
				EnableBattleMode(true);
		
				yield return new WaitUntil(() => IsCurrentNodeName(nodeIdleAction));
			}
		}

		private IEnumerator ExitIdleAction()
		{
			if (!humanoid.InBattle)
			{
				EnableBattleMode(false);
			
				yield return new WaitUntil(() => !IsCurrentNodeName(nodeIdleAction));
			}
		}


		private void OnJoinedBattle(SignalJoinBattleLocal signal)
		{
			EnableBattleMode(true);

			if (currentWeaponBehavior is DrawableWeapon drawable)
			{
				drawable.DrawWeapon();
			}
		}

		private void OnLeavedBattle(SignalLeaveBattleLocal signal)
		{
			EnableBattleMode(false);

			if (currentWeaponBehavior is DrawableWeapon drawable)
			{
				drawable.SheathWeapon();
			}
		}


		#region AnimationEvents
		private void AttackLeftHandEvent()
		{
			onAttackLeftHand?.Invoke();
		}
		private void AttackRightHandEvent()
		{
			onAttackRightHand?.Invoke();
		}
		private void AttackKickEvent()
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
	partial class HumanoidAnimatorController
	{
		private void SetBehaviorToUnArmed()
		{
			currentWeaponBehavior = new UnArmedBehavior(humanoid);
		}

		private void OnEquipWeaponChanged()
		{
			CharacterSheet sheet = humanoid.Sheet as CharacterSheet;

			Hands hands = sheet.Equipment.WeaponCurrent.Hands;

			var weaponMain = sheet.Equipment.WeaponCurrent.Main.Item?.GetItemData<WeaponItemData>();

			var lastBehavior = currentWeaponBehavior;

			if (hands == Hands.Main || hands == Hands.Spare || (hands == Hands.Both && weaponMain is MeleeItemData melee && melee.melleType == MelleType.OneHanded))
			{
				currentWeaponBehavior = new OneHandedBehavior(humanoid);
			}
			else if (hands == Hands.Both)
			{
				if (weaponMain is MeleeItemData)
				{
					currentWeaponBehavior = new TwoHandedBehavior(humanoid);
				}
				else if (weaponMain is RangedItemData)
				{
					currentWeaponBehavior = new RangedBehavior(humanoid);
				}
			}
			else
			{
				currentWeaponBehavior = new UnArmedBehavior(humanoid);
			}


			if (lastBehavior != null && lastBehavior != currentWeaponBehavior)
			{
				lastBehavior.Dispose();
			}

			if (humanoid.InBattle)
			{
				currentWeaponBehavior.UpdatePose();
			}
		}

		public abstract class WeaponBehavior
		{
			protected int weaponTypeHash;
			protected int attackTypeHash;

			protected IEquipment equipment;
			protected Animator animator;
			protected HumanoidAnimatorController animatorController;
			protected CharacterOutfit outfit;

			protected IBattlable owner;

			public WeaponBehavior(ICharacterModel owner)
			{
				this.owner = owner;
				equipment = (owner.Sheet as CharacterSheet).Equipment;
				outfit = (owner as CharacterModel).Outfit;
				animatorController = owner.AnimatorController as HumanoidAnimatorController;
				animator = animatorController.animator;

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

			protected DrawableWeapon(ICharacterModel owner) : base(owner)
			{
				drawTypeHash = Animator.StringToHash("DrawType");
				drawWeaponHash = Animator.StringToHash("DrawWeapon");
				sheathWeaponHash = Animator.StringToHash("SheathWeapon");

				animatorController.onDrawWeapon += OnWeaponDrawed;
				animatorController.onSheathWeapon += OnWeaponSheathed;

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

				animatorController.SetLayerWeightByName(animatorController.leftArmLayer, 0.7f);
				animatorController.SetLayerWeightByName(animatorController.rightArmLayer, 0f);

				animator.SetInteger(drawTypeHash, 1);//left hand
				animator.SetTrigger(drawWeaponHash);
			}
			protected virtual void SheathWeaponLeftHand()
			{
				isAnimated = true;

				animatorController.SetLayerWeightByName(animatorController.leftArmLayer, 0.7f);
				animatorController.SetLayerWeightByName(animatorController.rightArmLayer, 0f);

				animator.SetInteger(drawTypeHash, 1);//left hand
				animator.SetTrigger(sheathWeaponHash);
			}

			protected virtual void DrawWeaponRightHand()
			{
				isAnimated = true;

				animatorController.SetLayerWeightByName(animatorController.leftArmLayer, 0f);
				animatorController.SetLayerWeightByName(animatorController.rightArmLayer, 0.7f);

				animator.SetInteger(drawTypeHash, 0);//right hand
				animator.SetTrigger(drawWeaponHash);
			}
			protected virtual void SheathWeaponRightHand()
			{
				isAnimated = true;

				animatorController.SetLayerWeightByName(animatorController.leftArmLayer, 0f);
				animatorController.SetLayerWeightByName(animatorController.rightArmLayer, 0.7f);

				animator.SetInteger(drawTypeHash, 0);//right hand
				animator.SetTrigger(sheathWeaponHash);
			}

			protected virtual void DrawWeaponBothHand()
			{
				isAnimated = true;

				animatorController.SetLayerWeightByName(animatorController.leftArmLayer, 0.7f);
				animatorController.SetLayerWeightByName(animatorController.rightArmLayer, 0.7f);

				animator.SetInteger(drawTypeHash, 2);//both hands
				animator.SetTrigger(drawWeaponHash);
			}
			protected virtual void SheathWeaponBothHand()
			{
				isAnimated = true;

				animatorController.SetLayerWeightByName(animatorController.leftArmLayer, 0.7f);
				animatorController.SetLayerWeightByName(animatorController.rightArmLayer, 0.7f);

				animator.SetInteger(drawTypeHash, 2);//both hands
				animator.SetTrigger(sheathWeaponHash);
			}

			protected void AnimateLeftHand()
			{
				animatorController.SetLayerWeightByName(animatorController.leftArmLayer, owner.InBattle ? 0f : 0.7f);
				animatorController.SetLayerWeightByName(animatorController.leftHandLayer, 1f);
				animatorController.SetLayerWeightByName(animatorController.rightArmLayer, 0);
				animatorController.SetLayerWeightByName(animatorController.rightHandLayer, 0);
			}
			protected void AnimateRightHand()
			{
				animatorController.SetLayerWeightByName(animatorController.leftArmLayer, 0);
				animatorController.SetLayerWeightByName(animatorController.leftHandLayer, 0);
				animatorController.SetLayerWeightByName(animatorController.rightArmLayer, owner.InBattle ? 0f : 0.7f);
				animatorController.SetLayerWeightByName(animatorController.rightHandLayer, 1f);
			}
			protected void AnimateBothHands()
			{
				animatorController.SetLayerWeightByName(animatorController.leftArmLayer, owner.InBattle ? 0f : 0.7f);
				animatorController.SetLayerWeightByName(animatorController.leftHandLayer, 1f);
				animatorController.SetLayerWeightByName(animatorController.rightArmLayer, owner.InBattle ? 0f : 0.7f);
				animatorController.SetLayerWeightByName(animatorController.rightHandLayer, 1f);
			}

			public virtual void OnWeaponDrawed() { }
			public virtual void OnWeaponSheathed()
			{
				animatorController.SetLayerWeightByName(animatorController.rightArmLayer, 0f);
				animatorController.SetLayerWeightByName(animatorController.rightHandLayer, 0f);
				animatorController.SetLayerWeightByName(animatorController.leftArmLayer, 0f);
				animatorController.SetLayerWeightByName(animatorController.leftHandLayer, 0f);
			}
		}


		public class UnArmedBehavior : WeaponBehavior
		{
			public UnArmedBehavior(ICharacterModel owner) : base(owner)
			{
				UpdatePose();
			}

			public override void UpdatePose()
			{
				animatorController.SetLayerWeightByName(animatorController.leftArmLayer, 0f);
				animatorController.SetLayerWeightByName(animatorController.leftHandLayer, 0f);
				animatorController.SetLayerWeightByName(animatorController.rightArmLayer, 0f);
				animatorController.SetLayerWeightByName(animatorController.rightHandLayer, 0f);

				outfit.Slots.Clear();
			}

			public override void Attack()
			{
				animator.SetInteger(weaponTypeHash, 0);
				animator.SetInteger(attackTypeHash, Random.Range(0, 3));
				animator.SetTrigger(animatorController.attackHash);
			}

			public void Kick()
			{
				animator.SetInteger(weaponTypeHash, 0);
				animator.SetInteger(attackTypeHash, 2);//1 - skill
				animator.SetTrigger(animatorController.attackHash);
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

			public OneHandedBehavior(ICharacterModel owner) : base(owner)
			{
				useAnimationDraw = false;

				right = equipment.WeaponCurrent.Main.Item?.GetItemData<WeaponItemData>();
				left = equipment.WeaponCurrent.Spare.Item?.GetItemData<WeaponItemData>();

				UpdatePose();
			}

			public override void Dispose()
			{
				animatorController.onDrawWeapon -= OnWeaponDrawed;
				animatorController.onSheathWeapon -= OnWeaponSheathed;
			}


			public override void Attack()
			{
				animator.SetInteger(weaponTypeHash, 1);
				animator.SetInteger(attackTypeHash, Random.Range(0, 5));
				animator.SetTrigger(animatorController.attackHash);
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

			public TwoHandedBehavior(ICharacterModel owner) : base(owner)
			{
				data = equipment.WeaponCurrent.Main.Item.GetItemData<WeaponItemData>();

				UpdatePose();
			}

			public override void Dispose()
			{
				animatorController.onDrawWeapon -= OnWeaponDrawed;
				animatorController.onSheathWeapon -= OnWeaponSheathed;
			}


			public override void Attack()
			{
				animator.SetInteger(weaponTypeHash, 1);
				animator.SetInteger(attackTypeHash, 1);
				animator.SetTrigger(animatorController.attackHash);
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

			public RangedBehavior(ICharacterModel owner) : base(owner)
			{
				data = equipment.WeaponCurrent.Main.Item.GetItemData<WeaponItemData>();

				outfit.Slots.Clear();

				UpdatePose();
			}

			public override void Dispose()
			{
				animatorController.onDrawWeapon -= OnWeaponDrawed;
				animatorController.onSheathWeapon -= OnWeaponSheathed;
			}

			public override void Attack()
			{
				animator.SetInteger(weaponTypeHash, 3);
				animator.SetInteger(attackTypeHash, 0);
				animator.SetTrigger(animatorController.attackHash);
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
}