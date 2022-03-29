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

		Equip Weapon00 { get; }
		Equip Weapon01 { get; }
		Equip Weapon10 { get; }
		Equip Weapon11 { get; }

		List<Item> Items { get; }

		bool Add(Item item);
		bool Remove(Item item);
	}
}