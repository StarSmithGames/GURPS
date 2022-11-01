using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using Array2DEditor;

namespace Game.Systems.InventorySystem
{
    public interface IInventory
    {
        event UnityAction OnInventoryChanged;

        bool IsEmpty { get; }

        List<Slot> Slots { get; set; }

        List<Item> GetItems();

        bool Add(Item item, bool notify = true);

        bool Remove(Item item, bool notify = true);

        void Clear(bool notify = true);
    }

    public class Inventory : IInventory
    {
        public event UnityAction OnInventoryChanged;

        public bool IsEmpty => Slots.All((x) => x.IsEmpty);

        public List<Slot> Slots { get; set; }

        public Inventory(InventorySettings settings)
        {
            Slots = new List<Slot>();

            for (int i = 0; i < settings.slots.Count; i++)
            {
                Slot slot = settings.slots[i].Copy();
                slot.SetOwner(this);

                Slots.Add(slot);
            }
        }

        public List<Item> GetItems()
		{
            return Slots.Select((x) => x.Item).ToList();
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
                OnInventoryChanged?.Invoke();
            }

            void AddToFirstEmptySlot()
			{
                //find first empty slot
                Slot emptySlot = Slots.First((x) => x.IsEmpty);
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
			Slots.Find((x) => x.Item == item).SetItem(null);

			if (notify)
			{
                OnInventoryChanged?.Invoke();
            }
            return true;
        }

        public void Clear(bool notify = true)
        {
            //Items.Clear();
            if (notify)
			{
                OnInventoryChanged?.Invoke();
            }
        }

        public bool GetAllItemsByType(ItemData type, out List<Item> items)
        {
            items = Slots.Where((slot) => !slot.IsEmpty ? slot.Item.ItemData == type : false).Select((x) => x.Item).ToList();
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
    public class Slot : ICopyable<Slot>
	{
        public event UnityAction onChanged;

        public bool IsEmpty => Item?.ItemData == null;

        [HideLabel]
        public Item Item;

        public IInventory CurrentInventory { get; private set; }

        public void SetOwner(IInventory inventory)
        {
            CurrentInventory = inventory;
        }

        public bool SetItem(Item item)
        {
            if (item != null)
            {
                if (item.ItemData == null)
                {
                    item = null;
                }
            }

            Item = item;

            onChanged?.Invoke();

            return true;
        }

		public Slot Copy()
		{
            return new Slot()
            {
                Item = Item,
            };
		}

		private string Title => $"Slot with {Item.Title}";
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
        public List<Slot> slots = new List<Slot>();

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
                return new Slot();
            },
            () =>
			{
				if (slots.Any((x) => x.Item.ItemData == null))
				{
                    return slots.First((x) => x.Item.ItemData == null);
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