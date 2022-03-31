using Sirenix.OdinInspector;

using System.Linq;

using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public class UISlotEquipment : UISlot
	{
		[field: Space]
		[field: SerializeField] public Sprite BaseBackground { get; private set; }
		[field: SerializeField] public Sprite SwapBackground { get; private set; }

		public override bool IsEmpty => CurrentEquip?.IsEmpty ?? true;
		public Equip CurrentEquip { get; private set; }
		public override Item CurrentItem => CurrentEquip?.Item;

		public UIEquipment Owner { get; private set; }

		private Color one = Color.white;
		private Color oneHalfAlpha = new Color(1, 1, 1, 0.5f);

		public void SetOwner(UIEquipment owner)
		{
			Owner = owner;
		}

		public void SetEquip(Equip equip)
		{
			CurrentEquip = equip;
			CurrentEquip.onEquipChanged += UpdateUI;

			UpdateUI();
		}

		private void UpdateUI()
		{
			Icon.enabled = !CurrentEquip.IsEmpty;
			Icon.sprite = CurrentItem?.ItemData.itemSprite;
			Icon.color = CurrentEquip.Mark ? oneHalfAlpha : one;

			Background.sprite = CurrentEquip.IsEmpty ? SwapBackground : BaseBackground;
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