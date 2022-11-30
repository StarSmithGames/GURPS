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
		[field: SerializeField] public List<UISlotInventory> Slots { get; private set; }

		public Inventory CurrentInventory { get; private set; }

		public void SetInventory(Inventory inventory)
		{
			CurrentInventory = inventory;

			Assert.AreEqual(CurrentInventory.Slots.Count, Slots.Count, "Slots length not equal!");

			for (int i = 0; i < CurrentInventory.Slots.Count; i++)
			{
				Slots[i].SetSlot(CurrentInventory.Slots[i]);
			}
		}

		[Button(DirtyOnClick = true)]
		private void GetSlots()
		{
			Slots = Content.GetComponentsInChildren<UISlotInventory>().ToList();
		}
	}
}