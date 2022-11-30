using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Systems.InventorySystem
{
	public class UIEquipment : MonoBehaviour
	{
		[field: SerializeField] public UISlotEquipment Head { get; private set; }
		[field: SerializeField] public UISlotEquipment Sholders { get; private set; }
		[field: SerializeField] public UISlotEquipment Chest { get; private set; }
		[field: SerializeField] public UISlotEquipment Forearms { get; private set; }
		[field: SerializeField] public UISlotEquipment Legs { get; private set; }
		[field: SerializeField] public UISlotEquipment Feet { get; private set; }
		[field: SerializeField] public UISlotEquipment Cloak { get; private set; }
		[field: SerializeField] public UISlotEquipment Jewelry { get; private set; }
		[field: SerializeField] public UISlotEquipment Ring0 { get; private set; }
		[field: SerializeField] public UISlotEquipment Ring1 { get; private set; }
		[field: SerializeField] public UISlotEquipment Trinket { get; private set; }
		[field: Space]
		[field: SerializeField] public UISlotEquipment Weapon00 { get; private set; }
		[field: SerializeField] public UISlotEquipment Weapon01 { get; private set; }
		[field: SerializeField] public UISlotEquipment Weapon10 { get; private set; }
		[field: SerializeField] public UISlotEquipment Weapon11 { get; private set; }

		[field: SerializeField] public List<UISlotEquipment> Slots { get; private set; }

		public Equipment CurrentEquipment { get; private set; }

		public void SetEquipment(Equipment equipment)
		{
			CurrentEquipment = equipment;

			Assert.AreEqual(15, Slots.Count, "Slots length not equal!");

			Head.SetSlot(CurrentEquipment.Head);
			Sholders.SetSlot(CurrentEquipment.Sholders);
			Chest.SetSlot(CurrentEquipment.Chest);
			Forearms.SetSlot(CurrentEquipment.Forearms);
			Legs.SetSlot(CurrentEquipment.Legs);
			Feet.SetSlot(CurrentEquipment.Feet);
			Cloak.SetSlot(CurrentEquipment.Cloak);
			Jewelry.SetSlot(CurrentEquipment.Jewelry);
			Ring0.SetSlot(CurrentEquipment.Ring0);
			Ring1.SetSlot(CurrentEquipment.Ring1);
			Trinket.SetSlot(CurrentEquipment.Trinket);

			Weapon00.SetSlot(CurrentEquipment.WeaponMain.main);
			Weapon01.SetSlot(CurrentEquipment.WeaponMain.spare);
			Weapon10.SetSlot(CurrentEquipment.WeaponSpare.main);
			Weapon11.SetSlot(CurrentEquipment.WeaponSpare.spare);
		}

		[Button(DirtyOnClick = true)]
		private void GetSlots()
		{
			Slots = GetComponentsInChildren<UISlotEquipment>().ToList();
		}
	}
}