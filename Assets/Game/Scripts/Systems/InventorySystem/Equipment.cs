using System.Collections.Generic;
using UnityEngine.Events;

namespace Game.Systems.InventorySystem
{
	public class Equipment : IEquipment
	{
		public event UnityAction OnEquipmentChanged;

		public List<Item> Items { get; private set; }

		public bool Add(Item item)
		{
			return false;
		}

		public bool Remove(Item item)
		{
			return false;
		}
	}
}