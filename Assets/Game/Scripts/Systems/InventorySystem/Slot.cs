using Sirenix.OdinInspector;

using UnityEngine.Events;

namespace Game.Systems.InventorySystem
{
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
}