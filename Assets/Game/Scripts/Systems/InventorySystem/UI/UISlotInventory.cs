using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public class UISlotInventory : UISlot
	{
		[field: Space]
		[field: SerializeField] public TMPro.TextMeshProUGUI Count { get; private set; }

		public UIInventory Owner { get; private set; }

		public void SetOwner(UIInventory owner)
		{
			Owner = owner;
		}

		public void SetItem(Item item)
		{
			CurrentItem = item;

			UpdateUI();
		}

		private void UpdateUI()
		{
			Icon.enabled = CurrentItem != null;
			Icon.sprite = CurrentItem?.ItemData.information?.portrait;

			Count.enabled = CurrentItem != null && !CurrentItem.IsArmor && !CurrentItem.IsWeapon;
			Count.text = CurrentItem?.CurrentStackSize.ToString() ?? "";
		}
	}
}