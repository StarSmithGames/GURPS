using Game.Managers.PartyManager;
using Game.Systems.ContextMenu;

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class UISlotEquipment : UISlot<SlotEquipment>
	{
		[field: Space]
		[field: SerializeField] public Sprite BaseBackground { get; private set; }
		[field: SerializeField] public Sprite SwapBackground { get; private set; }
		[field: SerializeField] public Image Prohibition { get; private set; }

		public bool isWeaponSlot = false;
		[ShowIf("isWeaponSlot")]
		public bool isMainWeaponSlot = true;

		public Item Item => Slot.item;

		private bool IsWeaponSpareSlot => isWeaponSlot && !isMainWeaponSlot && !IsEmpty && Item.IsTwoHandedWeapon;

		private ContextMenuSystem contextMenuSystem;

		[Inject]
		private void Construct(ContextMenuSystem contextMenuSystem)
		{
			this.contextMenuSystem = contextMenuSystem;
		}

		private void Start()
		{
			containerHandler.Subscribe(this);
		}

		private void OnDestroy()
		{
			containerHandler.Unsubscribe(this);
		}

		public bool SetItem(Item item)
		{
			return Slot.SetItem(item);
		}

		protected override void UpdateUI()
		{
			Icon.enabled = !IsEmpty;
			Icon.sprite = Item?.ItemData?.information.portrait;
			Background.sprite = IsEmpty ? SwapBackground : BaseBackground;

			base.EnableHightlight(IsWeaponSpareSlot);
		}

		public void ContextMenu()
		{
			contextMenuSystem.SetTarget(Item);
		}

		public void EnableProhibition(bool trigger)
		{
			Prohibition.enabled = trigger;
		}

		//private void Swap()
		//{
		//	Background.sprite = Background.sprite == BaseBackground ? SwapBackground : BaseBackground;
		//}

		public override void Drop(UISlot slot)
		{
			EquipmentPointer.Drop(this, slot);
		}

		public override void EnableHightlight(bool trigger)
		{
			if (!IsWeaponSpareSlot)
			{
				base.EnableHightlight(trigger);
			}
		}
	}
}