using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems.InventorySystem
{
	public class UISlot : MonoBehaviour
	{
		public UISlotDragAndDropComponent DragAndDrop;

		public Image icon;
		public TMPro.TextMeshProUGUI count;

		public UIInventory Owner { get; private set; }
		public bool IsEmpty => CurrentItem == null;
		public Item CurrentItem { get; private set; }

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
			icon.enabled = CurrentItem != null;
			icon.sprite = CurrentItem?.ItemData.itemSprite;

			count.enabled = CurrentItem != null;
			count.text = CurrentItem?.CurrentStackSize.ToString() ?? "";
		}
	}
}