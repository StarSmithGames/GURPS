using System.Collections.Generic;
using UnityEngine.Events;

namespace Game.Systems.InventorySystem
{
	public interface IEquipment
	{
		event UnityAction OnEquipmentChanged;

		Equip Head { get; }
		Equip Sholders { get; }
		Equip Chest { get; }
		Equip Forearms { get; }
		Equip Legs { get; }
		Equip Feet { get; }

		EquipWeapon WeaponCurrent { get; }
		EquipWeapon WeaponMain { get; }
		EquipWeapon WeaponSpare { get; }

		Equip Cloak { get; }
		Equip Jewelry { get; }
		Equip Ring0 { get; }
		Equip Ring1 { get; }
		Equip Trinket { get; }

		bool Add(Item item);
		bool AddTo(Item item, Equip equip);

		bool RemoveFrom(Equip equip);

		bool Swap(Equip from, Equip to);
	}
}