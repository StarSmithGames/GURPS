using System.Collections.Generic;
using UnityEngine.Events;

namespace Game.Systems.InventorySystem
{
	public interface IEquipment
	{
		event UnityAction OnEquipmentChanged;

		List<Item> Items { get; }

		bool Add(Item item);
		bool Remove(Item item);
	}
}