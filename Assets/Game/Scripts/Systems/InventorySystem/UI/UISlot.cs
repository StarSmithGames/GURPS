using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public abstract class UISlot : PoolableObject
	{
		[field: SerializeField] public DragAndDropUISlotComponent DragAndDrop { get; private set; }

		[field: SerializeField] public Image Background { get; protected set; }
		[field: SerializeField] public Image Icon { get; protected set; }

		public Slot Slot { get; private set; }

		public virtual bool IsEmpty => Slot.IsEmpty;
		public Item CurrentItem => Slot.Item;

		protected ContainerSlotHandler containerHandler;

		[Inject]
		private void Construct(ContainerSlotHandler containerHandler)
		{
			this.containerHandler = containerHandler;
		}

		public void SetSlot(Slot slot)
		{
			if(Slot != null)
			{
				Slot.onChanged -= UpdateUI;
			}

			Slot = slot;

			UpdateUI();

			Slot.onChanged += UpdateUI;
		}

		public bool SetItem(Item item)
		{
			return Slot.SetItem(item);
		}

		public void Swap(UISlot slot)
		{
			Item item = slot.CurrentItem;
			slot.SetItem(CurrentItem);
			SetItem(item);
		}

		public abstract void Drop(UISlot slot);

		protected virtual void UpdateUI() { }
	}
}