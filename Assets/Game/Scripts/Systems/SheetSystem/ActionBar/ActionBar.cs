using Array2DEditor;
using Game.Systems.InventorySystem;

using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.Events;

namespace Game.Systems.SheetSystem
{
    public class ActionBar
    {
        public UnityAction onActionBarChanged;

        public List<SlotAction> Slots { get; }

        private ISheet sheet;

        public ActionBar(ActionBarSettings settings, ISheet sheet)
		{
            Slots = new List<SlotAction>();

			for (int i = 0; i < settings.slots.Count; i++)
			{
                Slots.Add(settings.slots[i].Copy());
			}

            this.sheet = sheet;

            sheet.Inventory.onInventoryChanged += OnInventoryChanged;
        }

        public bool Remove(IAction action, bool notify = false)
		{
            if (action == null) return false;

            var slot = Slots.Find((x) => x.action == action);
            if (slot != null)
            {
                slot.Dispose();

                if (notify)
                {
                    onActionBarChanged?.Invoke();
                }

                return true;
            }

            return false;
        }

        public bool IsContainsItem()
		{
            return Slots.Any((x) => x.action is Item);
        }

        private void OnInventoryChanged()
		{
			if (IsContainsItem())
			{
                var items = Slots.Select((x) => x.action as Item).ToArray();

				for (int i = 0; i < items.Length; i++)
				{
                    if (!sheet.Inventory.Contains(items[i]))
					{
                        Remove(items[i]);
                    }
                }
            }
		}
    }

    [System.Serializable]
    public class ActionBarSettings
	{
        [InfoBox("Toggle NOT work, use only for size.")]
        public Array2DBool grid;

        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Title")]
        public List<SlotAction> slots = new List<SlotAction>();

        [Button("Update Slots", DirtyOnClick = true)]
        private void UpdateSlots()
        {
            int size = grid.GridSize.x * grid.GridSize.y;
            CollectionExtensions.Resize(size, slots,
            () =>
            {
                return new SlotAction();
            },
            () =>
            {
                if (slots.Any((x) => x.action == null))
                {
                    return slots.First((x) => x.action == null);
                }
                return slots.Last();
            });
        }
    }
}