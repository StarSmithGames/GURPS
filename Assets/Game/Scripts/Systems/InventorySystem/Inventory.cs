using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using Array2DEditor;

namespace Game.Systems.InventorySystem
{
    public class Inventory
    {
        public event UnityAction onInventoryChanged;

        public bool IsEmpty => Slots.All((x) => x.IsEmpty);

        public List<SlotInventory> Slots { get; set; }

        public Inventory(InventorySettings settings)
        {
            Slots = new List<SlotInventory>();

            for (int i = 0; i < settings.slots.Count; i++)
            {
                SlotInventory slot = settings.slots[i].Copy();
                slot.SetOwner(this);

                Slots.Add(slot);
            }
        }

        public List<Item> GetItems()
		{
            return Slots.Select((x) => x.item).ToList();
        }

        public bool Add(Item item, bool notify = true)
        {
			if (item.ItemData.isStackable)
			{
				if (GetAllItemsByType(item.ItemData, out List<Item> items))
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

								onInventoryChanged?.Invoke();

								return true;
							}
							else
							{
								items[i].CurrentStackSize += item.CurrentStackSize;
								item.CurrentStackSize -= item.CurrentStackSize;

								onInventoryChanged?.Invoke();

								return true;
							}
						}
					}

                    AddToFirstEmptySlot();
                }
				else
				{
                    AddToFirstEmptySlot();
                }
            }
			else
			{
                AddToFirstEmptySlot();
            }

			if (notify)
			{
                onInventoryChanged?.Invoke();
            }

            void AddToFirstEmptySlot()
			{
                //find first empty slot
                SlotInventory emptySlot = Slots.First((x) => x.IsEmpty);
                if(emptySlot != null)
				{
                    emptySlot.SetItem(item);
				}
				else
				{
                    Debug.LogError("Can't Add Item");
				}
            }

            return true;
        }

        public bool Remove(Item item, bool notify = true)
        {
			Slots.Find((x) => x.item == item).SetItem(null);

			if (notify)
			{
                onInventoryChanged?.Invoke();
            }
            return true;
        }

        public bool Contains(Item item)
		{
            return Slots.Any((x) => x.item == item);
		}

        public void Clear(bool notify = true)
        {
            //Items.Clear();
            if (notify)
			{
                onInventoryChanged?.Invoke();
            }
        }

        public bool GetAllItemsByType(ItemData type, out List<Item> items)
        {
            items = Slots.Where((slot) => !slot.IsEmpty ? slot.item.ItemData == type : false).Select((x) => x.item).ToList();
            return items.Count > 0;
        }

        public Data GetData()
        {
            return new Data()
            {
                slots = Slots.ToArray(),
            };
        }

        public class Data
        {
            public Slot[] slots;
        }
    }

	[System.Serializable]
    public class InventorySettings
    {
        public bool useRandomItems = false;

        [ReadOnly] public bool isDynamicRows = false;

        [InfoBox("Toggle NOT work, use only for size.")]
        public Array2DBool grid;

        //sort by
        [HideIf("useRandomItems")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Title")]
        public List<SlotInventory> slots = new List<SlotInventory>();

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
                //for (int i = 0; i < items.Count; i++)
                //{
                //    if (items[i].useRandom)
                //    {
                //        result.Add(items[i].GenerateItem());
                //    }
                //    else
                //    {
                //        result.Add(items[i].Copy());
                //    }
                //}
            }

            //if (shuffleList)
            //{
            //    result = result.OrderBy(x => Guid.NewGuid()).ToList();
            //}

            return result;
        }
        
        [Button("Update Slots", DirtyOnClick = true)]
        private void UpdateSlots()
		{
            int size = grid.GridSize.x * grid.GridSize.y;
            CollectionExtensions.Resize(size, slots,
            () =>
			{
                return new SlotInventory();
            },
            () =>
			{
				if (slots.Any((x) => x.item.ItemData == null))
				{
                    return slots.First((x) => x.item.ItemData == null);
				}
                return slots.Last();
			});
        }
    }

    [System.Serializable]
    public class RandomInventorySettings
    {
        public bool isUnique = false;
        public bool useRandomCount = true;

        [HideIf("useRandomCount")]
        [MinValue(1), MaxValue(10)]
        public int itemsCount = 1;

        [ShowIf("useRandomCount")]
        [MinMaxSlider(1, 10)]
        public Vector2Int itemsMinMaxCount = new Vector2Int(1, 5);

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
        public List<ItemData> ignoreList = new List<ItemData>();
    }
}