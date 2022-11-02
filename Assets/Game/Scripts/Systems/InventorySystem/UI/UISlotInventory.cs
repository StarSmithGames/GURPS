using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public class UISlotInventory : UISlot
	{
		[field: Space]
		[field: SerializeField] public TMPro.TextMeshProUGUI Count { get; private set; }

		private void Start()
		{
			containerHandler.Subscribe(this);
		}

		private void OnDestroy()
		{
			containerHandler.Unsubscribe(this);
		}

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