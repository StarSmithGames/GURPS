using Game.Systems.InventorySystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public class UISlotAction : UISlot
	{
		[field: Space]
		[field: SerializeField] public TMPro.TextMeshProUGUI Count { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Num { get; private set; }

		protected override void UpdateUI()
		{
			bool isEmpty = IsEmpty;
			Icon.enabled = !isEmpty;
			Icon.sprite = !isEmpty ? CurrentItem.ItemData.information.portrait : null;

			Count.enabled = !isEmpty && !CurrentItem.IsArmor && !CurrentItem.IsWeapon;
			Count.text = !isEmpty ? CurrentItem.CurrentStackSize.ToString() : "";
		}
	}
}