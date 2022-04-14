using Game.Systems.InventorySystem;
using UnityEngine;

using Sirenix.OdinInspector;
using Game.Systems.SheetSystem;
using Zenject;

namespace Game.Entities
{
	public class CharacterOutfit : MonoBehaviour
	{
		public CharacterOutfitSlots Slots => slots;
		[SerializeField] private CharacterOutfitSlots slots;
		[Space]
		[SerializeField] private Transform equipmentContent;
		[SerializeField] private SkinnedMeshRenderer body;

		private Transform head;
		private Transform sholders;
		private Transform chest;
		private Transform forearms;
		private Transform legs;
		private Transform feet;

		private IEquipment equipment;

		[Inject]
		private void Construct(IEntity entity)
		{
			equipment = (entity.Sheet as CharacterSheet).Equipment;
		}

		private void Start()
		{
			head = CreateSlot();
			sholders = CreateSlot();
			chest = CreateSlot();
			forearms = CreateSlot();
			legs = CreateSlot();
			feet = CreateSlot();

			equipment.OnEquipmentChanged += OnEquipmentChanged;
		}

		private void OnDestroy()
		{
			if (equipment != null)
			{
				equipment.OnEquipmentChanged -= OnEquipmentChanged;
			}
		}

		private Transform CreateSlot()
		{
			Transform slot = new GameObject("_slot").transform;
			slot.SetParent(equipmentContent);

			return slot;
		}

		private void SetArmor(Transform root, Item item)
		{
			root.DestroyChildren();
			if(item != null && item.ItemData != null && item.ItemData.prefab != null)
			{
				var model = Instantiate(item.ItemData.prefab) as ItemArmorModel;
				model.transform.SetParent(root);
				model.Renderer.bones = body.bones;
				model.Renderer.rootBone = body.rootBone;

				model.Renderer.sharedMaterial = item.GetItemData<ArmorItemData>().baseMaterial;
			}
		}


		private void OnEquipmentChanged()
		{
			SetArmor(head, equipment.Head.Item);
			SetArmor(sholders, equipment.Sholders.Item);
			SetArmor(chest, equipment.Chest.Item);
			SetArmor(forearms, equipment.Forearms.Item);
			SetArmor(legs, equipment.Legs.Item);
			SetArmor(feet, equipment.Feet.Item);
		}
	}

	[System.Serializable]
	public class CharacterOutfitSlots
	{
		[InlineProperty]
		public OutfitWeaponSlot leftHand;
		[InlineProperty]
		public OutfitWeaponSlot rightHand;

		[InlineProperty]
		public OutfitWeaponSlot backSheath;

		[InlineProperty]
		public OutfitWeaponSlot leftSheath;
		[InlineProperty]
		public OutfitWeaponSlot rightSheath;

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
	public class OutfitWeaponSlot
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