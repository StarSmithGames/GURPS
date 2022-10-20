using Sirenix.OdinInspector;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class UIInventory : MonoBehaviour
	{
		[field: SerializeField] public Transform content;
		[field: SerializeField] public UISlotInventory[] slots;

		public IInventory CurrentInventory { get; private set; }

		private InventoryContainerHandler containerHandler;

		[Inject]
		private void Construct(InventoryContainerHandler containerHandler)
		{
			this.containerHandler = containerHandler;

			for (int i = 0; i < slots.Length; i++)
			{
				slots[i].SetOwner(this);
			}

			containerHandler?.Subscribe(slots);
		}

		private void OnDestroy()
		{
			containerHandler?.UnSubscribe(slots);
		}

		public void SetInventory(IInventory inventory)
		{
			if(CurrentInventory != null)
			{
				CurrentInventory.OnInventoryChanged -= OnInventoryChanged;
			}
			CurrentInventory = inventory;

			UpdateSlots();

			CurrentInventory.OnInventoryChanged += OnInventoryChanged;
		}

		private void UpdateSlots()
		{
			for (int i = 0; i < slots.Length; i++)
			{
				if (i < CurrentInventory.Items.Count)
				{
					slots[i].SetItem(CurrentInventory.Items[i]);
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