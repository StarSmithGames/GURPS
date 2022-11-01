using Game.Systems.CutomizationSystem;

using Sirenix.OdinInspector;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

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
		[field: Space]
		[field: SerializeField] public UISlotEquipment Weapon00 { get; private set; }
		[field: SerializeField] public UISlotEquipment Weapon01 { get; private set; }
		[field: SerializeField] public UISlotEquipment Weapon10 { get; private set; }
		[field: SerializeField] public UISlotEquipment Weapon11 { get; private set; }
		[field: Space]
		[field: SerializeField] public UISlotEquipment Cloak { get; private set; }
		[field: SerializeField] public UISlotEquipment Jewelry { get; private set; }
		[field: SerializeField] public UISlotEquipment Ring0 { get; private set; }
		[field: SerializeField] public UISlotEquipment Ring1 { get; private set; }
		[field: SerializeField] public UISlotEquipment Trinket { get; private set; }

		public UISlotEquipment[] slots;

		//public IEquipment CurrentEquipment => currentEquipment;
		//private IEquipment currentEquipment;

		private InventoryContainerHandler containerHandler;

		[Inject]
		private void Construct(InventoryContainerHandler containerHandler)
		{
			this.containerHandler = containerHandler;

			//for (int i = 0; i < slots.Length; i++)
			//{
			//	slots[i].SetOwner(this);
			//}

			//containerHandler.Subscribe(slots);
		}

		private void OnDestroy()
		{
			//containerHandler.UnSubscribe(slots);
		}

		//public void SetEquipment(IEquipment equipment)
		//{
		//	currentEquipment = equipment;

		//	Head.SetSlot(currentEquipment.Head);
		//	Sholders.SetSlot(currentEquipment.Sholders);
		//	Chest.SetSlot(currentEquipment.Chest);
		//	Forearms.SetSlot(currentEquipment.Forearms);
		//	Legs.SetSlot(currentEquipment.Legs);
		//	Feet.SetSlot(currentEquipment.Feet);

		//	Weapon00.SetSlot(currentEquipment.WeaponMain.Main);
		//	Weapon01.SetSlot(currentEquipment.WeaponMain.Spare);
		//	Weapon10.SetSlot(currentEquipment.WeaponSpare.Main);
		//	Weapon11.SetSlot(currentEquipment.WeaponSpare.Spare);

		//	Cloak.SetSlot(currentEquipment.Cloak);
		//	Jewelry.SetSlot(currentEquipment.Jewelry);
		//	Ring0.SetSlot(currentEquipment.Ring0);
		//	Ring1.SetSlot(currentEquipment.Ring1);
		//	Trinket.SetSlot(currentEquipment.Trinket);
		//}

		[Button]
		private void GetAllSlots()
		{
			slots = GetComponentsInChildren<UISlotEquipment>();
		}
	}
}