using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UnityEngine.Assertions;

namespace Game.Systems.InventorySystem
{
	public class UIInventory : MonoBehaviour
	{
		[field: SerializeField] public Transform Content { get; private set; }
		[field: SerializeField] public List<UISlot> Slots { get; private set; }

		public IInventory CurrentInventory { get; private set; }

		public void SetInventory(IInventory inventory)
		{
			CurrentInventory = inventory;

			CurrentInventory.Slots = new List<Slot>();

			for (int i = 0; i < Slots.Count; i++)
			{
				Slot slot = new Slot();
				slot.SetOwner(CurrentInventory);
				Slots[i].SetSlot(slot);
				
				CurrentInventory.Slots.Add(slot);
			}
		}

		[Button(DirtyOnClick = true)]
		private void GetAllSlots()
		{
			Slots = Content.GetComponentsInChildren<UISlot>().ToList();
		}
	}
}