using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public class UIEquipment : MonoBehaviour
	{
		public List<UISlotEquipment> slots = new List<UISlotEquipment>();

		[Button]
		private void GetAllSlots()
		{
			slots = GetComponentsInChildren<UISlotEquipment>().ToList();
		}
	}
}