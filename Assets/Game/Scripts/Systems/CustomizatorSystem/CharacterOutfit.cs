using Game.Systems.InventorySystem;
using UnityEngine;

using Sirenix.OdinInspector;
using Game.Systems.SheetSystem;
using Zenject;

using DG.Tweening;

namespace Game.Entities
{
	public class CharacterOutfit : MonoBehaviour
	{
		public Transform LeftHandPivot;
		[Space]
		[SerializeField] private CharacterOutfitSlots slots;
		public CharacterOutfitSlots Slots => slots;
		[Space]
		[SerializeField] private Transform equipmentContent;
		[SerializeField] private SkinnedMeshRenderer body;

		private Transform head;
		private Transform sholders;
		private Transform chest;
		private Transform forearms;
		private Transform legs;
		private Transform feet;

		//private IEquipment equipment;

		[Inject]
		private void Construct()
		{
			//equipment = (entity.Sheet as CharacterSheet).Equipment;
		}

		private void Start()
		{
			head = CreateSlot();
			sholders = CreateSlot();
			chest = CreateSlot();
			forearms = CreateSlot();
			legs = CreateSlot();
			feet = CreateSlot();

			//equipment.OnEquipmentChanged += OnEquipmentChanged;
		}

		private void OnDestroy()
		{
			//if (equipment != null)
			//{
			//	equipment.OnEquipmentChanged -= OnEquipmentChanged;
			//}
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
			//SetArmor(head, equipment.Head.CurrentItem);
			//SetArmor(sholders, equipment.Sholders.CurrentItem);
			//SetArmor(chest, equipment.Chest.CurrentItem);
			//SetArmor(forearms, equipment.Forearms.CurrentItem);
			//SetArmor(legs, equipment.Legs.CurrentItem);
			//SetArmor(feet, equipment.Feet.CurrentItem);
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

		public void Set(Transform obj, Transform3 offset, bool isAnimated = false)
		{
			obj.SetParent(slot);

			if (isAnimated)
			{
				obj.DOLocalMove(offset.position, 0.25f);
				obj.DOLocalRotate(offset.rotation.eulerAngles, 0.25f);
			}
			else
			{
				obj.localPosition = offset.position;
				obj.localRotation = offset.rotation;
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
[System.Serializable]
public struct Transform3
{
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;
}