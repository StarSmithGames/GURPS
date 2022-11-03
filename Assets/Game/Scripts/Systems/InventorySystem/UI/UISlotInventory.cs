using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public class UISlotInventory : UISlot<SlotInventory>
	{
		[field: Space]
		[field: SerializeField] public TMPro.TextMeshProUGUI Count { get; private set; }

		public Item Item => Slot.item;

		private void Start()
		{
			containerHandler.Subscribe(this);
		}

		private void OnDestroy()
		{
			containerHandler.Unsubscribe(this);
		}

		public bool SetItem(Item item)
		{
			return Slot.SetItem(item);
		}

		public void Swap(UISlotInventory slot)
		{
			Item item = slot.Slot.item;
			slot.Slot.SetItem(Slot.item);
			Slot.SetItem(item);
		}

		public override void Drop(UISlot slot)
		{
			InventoryDrop.Process(this, slot);
		}

		protected override void UpdateUI()
		{
			bool isEmpty = Slot.IsEmpty;
			Icon.enabled = !isEmpty;
			Icon.sprite = !isEmpty ? Slot.item.ItemData.information.portrait : null;

			Count.enabled = !isEmpty && !Slot.item.IsArmor && !Slot.item.IsWeapon;
			Count.text = !isEmpty ? Slot.item.CurrentStackSize.ToString() : "";
		}
	}
}