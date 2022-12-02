using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems.InventorySystem
{
	public class UISlotEquipment : UISlot<SlotEquipment>
	{
		[field: Space]
		[field: SerializeField] public Sprite BaseBackground { get; private set; }
		[field: SerializeField] public Sprite SwapBackground { get; private set; }
		[field: SerializeField] public Image Prohibition { get; private set; }

		public bool Mark { get; set; }
		public Item Item => Slot.item;

		private Color one = Color.white;
		private Color oneHalfAlpha = new Color(1, 1, 1, 0.5f);

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
			Icon.color = Mark ? oneHalfAlpha : one;

			Background.sprite = IsEmpty ? SwapBackground : BaseBackground;
		}

		public void EnableProhibition(bool trigger)
		{
			Prohibition.enabled = trigger;
		}

		private void Swap()
		{
			Background.sprite = Background.sprite == BaseBackground ? SwapBackground : BaseBackground;
		}

		public override void Drop(UISlot slot)
		{
			EquipmentPointer.Drop(this, slot);
		}
	}
}