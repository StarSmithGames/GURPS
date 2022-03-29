using Game.Systems.CharacterCutomization;

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

		public UISlotEquipment[] slots;

		public IEquipment CurrentEquipment => currentEquipment;
		private IEquipment currentEquipment;

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
			containerHandler.UnSubscribe(slots);
		}

		public void SetEquipment(IEquipment equipment)
		{
			if (currentEquipment != null)
			{
				currentEquipment.OnEquipmentChanged -= OnInventoryChanged;
			}
			currentEquipment = equipment;

			Head.SetEquip(currentEquipment.Head);
			Sholders.SetEquip(currentEquipment.Sholders);
			Chest.SetEquip(currentEquipment.Chest);
			Forearms.SetEquip(currentEquipment.Forearms);
			Legs.SetEquip(currentEquipment.Legs);
			Feet.SetEquip(currentEquipment.Feet);

			Weapon00.SetEquip(currentEquipment.Weapon00);
			Weapon01.SetEquip(currentEquipment.Weapon01);
			Weapon10.SetEquip(currentEquipment.Weapon10);
			Weapon11.SetEquip(currentEquipment.Weapon11);
		}

		private void UpdateSlots()
		{
			foreach (var item in currentEquipment.Items)
			{
			}
		}

		private void OnInventoryChanged()
		{
			UpdateSlots();
		}

		[Button]
		private void GetAllSlots()
		{
			slots = GetComponentsInChildren<UISlotEquipment>();
		}
	}
}