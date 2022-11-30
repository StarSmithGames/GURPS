using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public class UISlotEquipment : UISlot<SlotEquipment>
	{
		public bool Mark { get; set; }

		[field: Space]
		[field: SerializeField] public Sprite BaseBackground { get; private set; }
		[field: SerializeField] public Sprite SwapBackground { get; private set; }

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
			//Icon.enabled = !IsEmpty;
			//Icon.sprite = CurrentItem?.ItemData.information?.portrait;
			//Icon.color = Mark ? oneHalfAlpha : one;

			//Background.sprite = IsEmpty ? SwapBackground : BaseBackground;
		}

		private void Swap()
		{
			Background.sprite = Background.sprite == BaseBackground ? SwapBackground : BaseBackground;
		}

		public override void Drop(UISlot slot)
		{
		}
	}
}