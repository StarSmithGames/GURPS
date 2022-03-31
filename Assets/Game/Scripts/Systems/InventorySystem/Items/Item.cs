using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

namespace Game.Systems.InventorySystem
{
	[System.Serializable]
	public class Item : ICopyable<Item>
	{
		public UnityAction OnItemChanged;

		[ShowIf("IsCanBeRandom")]
		public bool useRandom = false;
		private bool IsCanBeRandom => IsStackable || IsBreakable || IsWeapon;


		public ItemData ItemData => data;
		[Required]
		[OnValueChanged("OnDataChanged")]
		[SerializeField] private ItemData data;

		[ShowIf("@IsStackable && !useRandom")]
		[MinValue("MinimumStackSize"), MaxValue("MaximumStackSize")]
		[SerializeField] private int currentStackSize = 1;
		public int CurrentStackSize
		{
			get => currentStackSize;
			set
			{
				currentStackSize = value;

				OnItemChanged?.Invoke();
			}
		}
		public int MaximumStackSize => IsInfinityStack ? 9999 : (data?.stackSize ?? 1);
		public int MinimumStackSize => 1;

		public bool IsStackSizeFull => CurrentStackSize == MaximumStackSize;
		public bool IsStackSizeEmpty => CurrentStackSize == 0;

		public int StackDifference => MaximumStackSize - CurrentStackSize;

		public bool IsStackable => data?.isStackable ?? false;
		public bool IsInfinityStack => data?.isInfinityStack ?? false;


		[InfoBox("@WeightInfo")]
		[HideIf("@useRandom")]
		[MinValue("MinimumWeight"), MaxValue("MaximumWeight")]
		[SuffixLabel("kg", true)]
		[ReadOnly] [SerializeField] private float currentWeight = 0.01f;
		public float CurrentWeight
		{
			get => currentWeight;
			set
			{
				currentWeight = value;

				OnItemChanged?.Invoke();
			}
		}
		public float CurrentWeightRounded => (float)System.Math.Round(CurrentWeight, 2);

		public float MaximumWeight => data?.weight ?? 99.99f;
		public float MinimumWeight => 0.01f;


		[ShowIf("@IsBreakable && !useRandom")]
		[MinValue("MinimumDurability"), MaxValue("MaximumDurability")]
		[SuffixLabel("%", true)]
		[SerializeField] private float currentDurability = 100f;
		public float CurrentDurability
		{
			get => currentDurability;
			set
			{
				currentDurability = value;

				OnItemChanged?.Invoke();
			}
		}
		public float MaximumDurability => 100f;
		public float MinimumDurability => 0;

		public bool IsBreakable => data?.isBreakable ?? false;

		public bool IsEquippable => data != null && data is EquippableItemData;
		public bool IsArmor => data != null && data is ArmorItemData;
		public bool IsWeapon => data != null && data is WeaponItemData;
		public bool IsTwoHandedWeapon
		{
			get
			{
				if(data != null)
				{
					if(ItemData is MeleeItemData meleeItemData)
					{
						if (meleeItemData.melleType == MelleType.TwoHanded)
						{
							return true;
						}
					}
					if(ItemData is RangedItemData)
					{
						return true;
					}
				}

				return false;
			}
		}

		public Item GenerateItem()//rnd item
		{
			return null;
		}

		public Item Copy()
		{
			return new Item()
			{
				data = data,
				currentStackSize = CurrentStackSize,
				currentWeight = CurrentWeight,
				currentDurability = CurrentDurability,
			};
		}


		private string Tittle
		{
			get
			{
				if (data != null && data.localizations.Count > 0)
				{
					return data.GetLocalization().itemName;
				}

				return "";
			}
		}

		private string WeightInfo => "Weight == " + currentStackSize * MaximumWeight;

		private void OnDataChanged()
		{
			currentStackSize = MaximumStackSize;
			currentWeight = MaximumWeight;
		}
	}
}