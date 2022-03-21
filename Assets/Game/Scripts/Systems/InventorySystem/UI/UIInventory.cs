using Sirenix.OdinInspector;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public class UIInventory : MonoBehaviour
	{
		public Transform content;
		public List<UISlot> slots = new List<UISlot>();

		private InventoryContainerHandler containerHandler;

		private void Construct(InventoryContainerHandler containerHandler)
		{
			this.containerHandler = containerHandler;

			containerHandler.Subscribe(this);
		}

		private void OnDestroy()
		{
			containerHandler?.UnSubscribe(this);
		}

		public void SetInventory(IInventory inventory)
		{
			for (int i = 0; i < slots.Count; i++)
			{
				if (i < inventory.Items.Count)
				{
					slots[i].SetItem(inventory.Items[i]);
				}
				else
				{
					slots[i].SetItem(null);
				}
			}
		}


		[Button]
		private void GetAllSlots()
		{
			slots = GetComponentsInChildren<UISlot>().ToList();
		}
	}
}