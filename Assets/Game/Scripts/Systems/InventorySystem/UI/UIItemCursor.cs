using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems.InventorySystem
{
	public class UIItemCursor : MonoBehaviour
	{
		[SerializeField] private Image icon;

		public UISlot Slot { get; private set; }
		public Item Item { get; private set; }

		private Transform parent;

		private void Awake()
		{
			parent = transform.parent;
		}

		public void SetSlot(UISlot slot)
		{
			Slot = slot;
		}

		public void SetItem(Item item)
		{
			Item = item;

			icon.sprite = item?.ItemData.itemSprite;
		}

		public void Dispose()
		{
			transform.parent = parent;
			Slot = null;
			Item = null;
			icon.sprite = null;
		}
	}
}