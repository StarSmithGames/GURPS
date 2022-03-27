using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public class UISlotEquipment : UISlot
	{
		[field: Space]
		[field: SerializeField] public Sprite BaseBackground { get; private set; }
		[field: SerializeField] public Sprite SwapBackground { get; private set; }

		public bool IsEmpty => CurrentItem == null;
		public Item CurrentItem { get; private set; }

		public UIEquipment Owner { get; private set; }

		public void SetOwner(UIEquipment owner)
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
			Icon.sprite = CurrentItem?.ItemData.itemSprite;
		}


		[Button]
		private void Swap()
		{
			Background.sprite = Background.sprite == BaseBackground ? SwapBackground : BaseBackground;
		}
	}
}