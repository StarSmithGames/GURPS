using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public class UISlotEquipment : UISlot<Slot>
	{
		public bool Mark { get; set; }

		[field: Space]
		[field: SerializeField] public Sprite BaseBackground { get; private set; }
		[field: SerializeField] public Sprite SwapBackground { get; private set; }

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

		protected override void UpdateUI()
		{
			//Icon.enabled = !IsEmpty;
			//Icon.sprite = CurrentItem?.ItemData.information?.portrait;
			//Icon.color = Mark ? oneHalfAlpha : one;

			//Background.sprite = IsEmpty ? SwapBackground : BaseBackground;
		}

		[Button(DirtyOnClick = true)]
		private void Swap()
		{
			Background.sprite = Background.sprite == BaseBackground ? SwapBackground : BaseBackground;
		}

		public override void Drop(UISlot slot)
		{
			throw new System.NotImplementedException();
		}
	}
}