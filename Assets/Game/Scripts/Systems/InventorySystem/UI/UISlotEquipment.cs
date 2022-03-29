using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public class UISlotEquipment : UISlot
	{
		[field: Space]
		[field: SerializeField] public Sprite BaseBackground { get; private set; }
		[field: SerializeField] public Sprite SwapBackground { get; private set; }

		public Equip CurrentEqup { get; private set; }

		public UIEquipment Owner { get; private set; }

		public void SetOwner(UIEquipment owner)
		{
			Owner = owner;
		}

		public void SetEquip(Equip equip)
		{
			CurrentEqup = equip;
			CurrentEqup.onEquipChanged += UpdateUI;

			UpdateUI();
		}

		private void UpdateUI()
		{
			Icon.enabled = !CurrentEqup.IsEmpty;
			Icon.sprite = CurrentEqup.Item?.ItemData.itemSprite;

			Background.sprite = !CurrentEqup.IsEmpty ? SwapBackground : BaseBackground;
		}

		[Button]
		private void Swap()
		{
			Background.sprite = Background.sprite == BaseBackground ? SwapBackground : BaseBackground;
#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
		}
	}
}