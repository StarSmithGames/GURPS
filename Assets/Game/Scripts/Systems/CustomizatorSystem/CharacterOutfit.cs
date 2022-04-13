using Game.Systems.InventorySystem;
using UnityEngine;

using Sirenix.OdinInspector;

namespace Game.Entities
{
	public class CharacterOutfit : MonoBehaviour
	{
		public CharacterOutfitSlots Slots => slots;
		[SerializeField] private CharacterOutfitSlots slots;
	}

	[System.Serializable]
	public class CharacterOutfitSlots
	{
		[InlineProperty]
		public OutfitSlot leftHand;
		[InlineProperty]
		public OutfitSlot rightHand;

		[InlineProperty]
		public OutfitSlot backSheath;

		[InlineProperty]
		public OutfitSlot leftSheath;
		[InlineProperty]
		public OutfitSlot rightSheath;

		public void Clear()
		{
			leftHand.Clear();
			rightHand.Clear();
			backSheath.Clear();
			leftSheath.Clear();
			rightSheath.Clear();
		}
	}


	[System.Serializable]
	public class OutfitSlot
	{
		[HideLabel]
		public Transform slot;

		public void Replace(ItemModel model, OutfitSlotOffset? offset = null)
		{
			Clear();

			if (model != null)
			{
				var obj = GameObject.Instantiate(model, slot);
				
				if(model.Item.ItemData is WeaponItemData)
				{
					obj.transform.localPosition = offset?.position ?? Vector3.zero;
					obj.transform.localRotation = offset?.rotation ?? Quaternion.identity;
				}
			}
		}

		public void Clear()
		{
			slot.DestroyChildren();
		}
	}

	[System.Serializable]
	public struct OutfitSlotOffset
	{
		public Vector3 position;
		public Quaternion rotation;
	}
}