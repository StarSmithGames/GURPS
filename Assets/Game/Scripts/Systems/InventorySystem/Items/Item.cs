using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using Game.Managers.StorageManager;
using Game.Systems.SheetSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Systems.InventorySystem
{
	[System.Serializable]
	public class Item : IAction, ICopyable<Item>
	{
		public UnityAction OnItemChanged;

		public ItemData ItemData => data;
		[SerializeField] private ItemData data;

		public bool useRandom = false;
		public bool IsCanBeRandom => IsStackable /*|| IsWeighty*/ || IsBreakable;

		#region StackSize
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

		[ShowIf("useRandom")]
		[SerializeField] private float minimumStackSizeRandom;
		[ShowIf("useRandom")]
		[SerializeField] private float maximumStackSizeRandom;

		public int TryAdd(Item item)
		{
			if (IsStackable)
			{
				if(item.ItemData == data)
				{
					if (!IsStackSizeFull)
					{

					}
				}
			}

			return 0;
		}

		#endregion

		#region Weight
		[SerializeField] private float currentWeight = 0.01f;
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

		public bool IsWeighty => data?.isWeighty ?? false;
		#endregion

		#region Durability
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
		#endregion

		public bool IsConsumable => data != null && data is ConsumableItemData;
		public bool IsEatable => data != null && data is FoodItemData;
		public bool IsDrinkable => data != null && data is DrinkItemData;

		public bool IsEquippable => data != null && data is EquippableItemData;
		public bool IsArmor => data != null && data is ArmorItemData;
		public bool IsWeapon => data != null && data is WeaponItemData;
		public bool IsMelleWeapon
		{
			get
			{
				if (data != null)
				{
					return ItemData is MeleeItemData;
				}

				return false;
			}
		}
		public bool IsRangedWeapon
		{
			get
			{
				if (data != null)
				{
					return ItemData is RangedItemData;
				}

				return false;
			}
		}
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

		public T GetItemData<T>() where T : ItemData => data as T;

		public void Randomize()
		{
			if (IsCanBeRandom && useRandom)
			{
				if (data != null)
				{
					if (data.isStackable)
					{
						CurrentStackSize = (int)Random.Range(minimumStackSizeRandom, maximumStackSizeRandom);
					}
				}
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

		public override string ToString()
		{
			Assert.IsNotNull(data);

			return $"{data?.information.GetName() ?? "NULL Data"} {(data != null ? (data.isStackable ? $" x{CurrentStackSize}" : "") : "")}";
		}

		public string Title => data?.GetName() ?? "NULL Data";


		/// GUI use in Nodes
#if UNITY_EDITOR
		[HideInInspector]
		public bool isShowFoldout = false;

		public void OnGUI()
		{
			var oldData = data;
			var oldRandom = useRandom;

			data = (ItemData)EditorGUILayout.ObjectField("Data", data, typeof(ItemData), true);
			if (IsCanBeRandom)
			{
				useRandom = EditorGUILayout.Toggle("Use Random?", useRandom);
			}
			else
			{
				useRandom = false;
			}

			if (!useRandom)
			{
				//Data Changed
				if (data != oldData)
				{
					currentStackSize = Mathf.Clamp(MaximumStackSize / 2, MinimumStackSize, MaximumStackSize);
					currentWeight = data.weight;
					currentDurability = MaximumDurability;
				}

				if (data != null)
				{
					if (data.isStackable)
					{
						currentStackSize = EditorGUILayout.IntSlider($"Stack Size {(data.isInfinityStack ? "inf" : "")}", currentStackSize, MinimumStackSize, MaximumStackSize);
					}

					if (data.isWeighty)
					{
						EditorGUILayout.LabelField($"Current Weight = {MaximumWeight} * {currentStackSize} == {MaximumWeight * currentStackSize}");
						EditorGUILayout.HelpBox("Weight == CurrentWeight * StackSize", MessageType.Info);
					}

					if (data.isBreakable)
					{
						currentDurability = EditorGUILayout.Slider("Durability %", currentDurability, MinimumDurability, MaximumDurability);
					}
				}
			}
			else
			{
				if (oldRandom != useRandom)
				{
					minimumStackSizeRandom = Mathf.Clamp(MaximumStackSize / 2, MinimumStackSize, MaximumStackSize);
					maximumStackSizeRandom = MaximumStackSize;
				}

				if (data != null)
				{
					if (data.isStackable)
					{
						GUILayout.Label($"StackSize: {MinimumStackSize} - {MaximumStackSize}");
						GUILayout.BeginHorizontal();
						minimumStackSizeRandom = EditorGUILayout.IntField("MinMax Stack Size", (int)minimumStackSizeRandom, GUILayout.MaxWidth(200));
						EditorGUILayout.MinMaxSlider(ref minimumStackSizeRandom, ref maximumStackSizeRandom, MinimumStackSize, MaximumStackSize);
						maximumStackSizeRandom = EditorGUILayout.IntField((int)maximumStackSizeRandom, GUILayout.MaxWidth(50));
						GUILayout.EndHorizontal();
					}
				}
			}
		}

		public static void OnGUIList(List<Item> items)
		{
			if (GUILayout.Button("Add Item"))
			{
				items.Add(new Item());
			}

			ParadoxNotion.Design.EditorUtils.ReorderableList(items, (i, picked) =>
			{
				var item = items[i];
				bool lastFoldout = item.isShowFoldout;

				GUILayout.BeginHorizontal("box");

				var text = string.Format("{0} {1} {2}", item.isShowFoldout ? "-" : "+", $"[{i + 1}]", item.data?.name ?? "Empty");
				if (GUILayout.Button(text, (GUIStyle)"label", GUILayout.Width(0), GUILayout.ExpandWidth(true)))
				{
					item.isShowFoldout = !item.isShowFoldout;
				}

				if (GUILayout.Button("X", GUILayout.Width(20)))//remove
				{
					items.RemoveAt(i);
				}

				GUILayout.EndHorizontal();

				if (item.isShowFoldout)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Space(10f);
					GUILayout.BeginVertical();
					item?.OnGUI();
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
				else
				{
					if (lastFoldout != item.isShowFoldout)//close all
					{

					}
				}
			});
		}
#endif
	}
}