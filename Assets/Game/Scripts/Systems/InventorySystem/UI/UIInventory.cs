using Sirenix.OdinInspector;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class UIInventory : MonoBehaviour
	{
		public Transform content;
		public UISlotInventory[] slots;

		public IInventory CurrentInventory => currentInventory;
		private IInventory currentInventory;

		private InventoryContainerHandler containerHandler;

		[Inject]
		private void Construct(InventoryContainerHandler containerHandler)
		{
			this.containerHandler = containerHandler;

			for (int i = 0; i < slots.Length; i++)
			{
				slots[i].SetOwner(this);
			}

			containerHandler.Subscribe(slots);
		}

		private void OnDestroy()
		{
			containerHandler?.UnSubscribe(slots);
		}

		public void SetInventory(IInventory inventory)
		{
			if(currentInventory != null)
			{
				currentInventory.OnInventoryChanged -= OnInventoryChanged;
			}
			currentInventory = inventory;

			UpdateSlots();

			currentInventory.OnInventoryChanged += OnInventoryChanged;
		}

		private void UpdateSlots()
		{
			for (int i = 0; i < slots.Length; i++)
			{
				if (i < currentInventory.Items.Count)
				{
					slots[i].SetItem(currentInventory.Items[i]);
				}
				else
				{
					slots[i].SetItem(null);
				}
			}
		}

		private void OnInventoryChanged()
		{
			UpdateSlots();
		}


		[Button]
		private void GetAllSlots()
		{
			slots = GetComponentsInChildren<UISlotInventory>();
		}
	}
}