using Sirenix.OdinInspector;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

namespace Game.Systems.InventorySystem
{
	public class Inventory : IInventory
    {
        public event UnityAction OnInventoryChanged;

        public bool IsEmpty => Items.Count == 0;

        public List<Item> Items { get; private set; }

        public Inventory()
		{
            Items = new List<Item>();
        }

		public Inventory(InventorySettings settings)
        {
            Items = settings.GenerateItems();
        }

        public bool Add(Item item)
        {
            if (item.ItemData.isStackable)
            {
                if (GetAllByData(item.ItemData, out List<Item> items))
                {
                    int currentStackSize = item.CurrentStackSize;

                    for (int i = 0; i < items.Count; i++)
                    {
                        if (!items[i].IsStackSizeFull)
                        {
                            int otherStackDifference = items[i].StackDifference;

                            int diff = currentStackSize - otherStackDifference;

                            if (diff > 0)
                            {
                                item.CurrentStackSize -= (currentStackSize - diff);
                                items[i].CurrentStackSize += (currentStackSize - diff);

                                if (item.IsStackSizeEmpty) return true;
                            }
                            else if (diff == 0)
                            {
                                item.CurrentStackSize -= currentStackSize;
                                items[i].CurrentStackSize += currentStackSize;

                                OnInventoryChanged?.Invoke();

                                return true;
                            }
                            else
                            {
                                items[i].CurrentStackSize += item.CurrentStackSize;
                                item.CurrentStackSize -= item.CurrentStackSize;

                                OnInventoryChanged?.Invoke();

                                return true;
                            }
                        }
                    }

                    Items.Add(item);
                }
                else
                {
                    Items.Add(item);
                }
            }
            else
            {
                Items.Add(item);
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool Remove(Item item)
        {
            Items.Remove(item);
            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool GetAllByData(ItemData data, out List<Item> items)
        {
            items = Items.Where((item) => item.ItemData == data).ToList();

            return items.Count > 0;
        }

		public void Clear()
		{
            Items.Clear();
            OnInventoryChanged?.Invoke();
        }
	}

    [System.Serializable]
    public class InventorySettings
    {
        public bool useRandomItems = true;
        public bool shuffleList = true;
        //sort by

        [HideIf("useRandomItems")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Tittle")]
        public List<Item> items = new List<Item>();

        [ShowIf("useRandomItems")]
        [InlineProperty]
        public RandomInventorySettings randomSettings;

        public List<Item> GenerateItems()
        {
            List<Item> result = new List<Item>();

            if (useRandomItems)
            {

            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].useRandom)
                    {
						result.Add(items[i].GenerateItem());
					}
                    else
                    {
						result.Add(items[i].Copy());
					}
                }
            }

            if (shuffleList)
            {
				result = result.OrderBy(x => Guid.NewGuid()).ToList();
			}

            return result;
        }
    }

    [System.Serializable]
    public class RandomInventorySettings
    {
        [Tooltip("Items ?? ???????????.")]
        public bool isUnique = false;
        public bool useRandomCount = true;

        [HideIf("useRandomCount")]
        [MinValue(1), MaxValue(10)]
        public int itemsCount = 1;

        [ShowIf("useRandomCount")]
        [MinMaxSlider(1, 10)]
        public Vector2Int itemsMinMaxCount = new Vector2Int(1, 5);

        [Tooltip("Items ??????? ????? ????? ???????? ?????????????.(????? ????????? ? ???????.)")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Tittle")]
        public List<Item> staticItems = new List<Item>();

        #region Bools
        [ToggleGroup("useConsumableItems", "Use Consumable Items")]
        public bool useConsumableItems = true;

        [ToggleGroup("useFireItems", "Use Fire Items")]
        public bool useFireItems = true;
        [ToggleGroup("useFireItems")]
        [ToggleLeft]
        public bool useAccelerantItems = true;
        [ToggleGroup("useFireItems")]
        [ToggleLeft]
        public bool useFuelItems = true;
        [ToggleGroup("useFireItems")]
        [ToggleLeft]
        public bool useStarterItems = true;
        [ToggleGroup("useFireItems")]
        [ToggleLeft]
        public bool useTinderItems = true;

        [ToggleGroup("useToolItems", "Use Tool Items")]
        public bool useToolItems = true;

        [ToggleGroup("useMaterialItems", "Use Material Items")]
        public bool useMaterialItems = true;
        #endregion

        [Space]
        [Tooltip("???? ?????? ??????? ????? ?????????????? ?????????????.")]
        public List<ItemData> ignoreList = new List<ItemData>();
    }
}