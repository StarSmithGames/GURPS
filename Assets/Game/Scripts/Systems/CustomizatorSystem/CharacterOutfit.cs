using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using UnityEngine;

using Zenject;

namespace Game.Entities
{
	public class CharacterOutfit : MonoBehaviour
	{
		public bool WeaponInSheath = false;
		public Hands BusyHands { get; private set; }

		[SerializeField] private Transform leftHand;
		[SerializeField] private Transform rightHand;

		private IEquipment equipment;

		[Inject]
		private void Construct(IEntity entity)
		{
			equipment = ((entity as Character).Sheet as CharacterSheet).Equipment;
		}

		private void OnDestroy()
		{
			if (equipment != null)
			{
				equipment.WeaponMain.onEquipWeaponChanged -= OnEquipWeaponChanged;
			}
		}

		private void Start()
		{
			equipment.WeaponMain.onEquipWeaponChanged += OnEquipWeaponChanged;

			OnEquipWeaponChanged();
		}

		private void OnEquipWeaponChanged()
		{
			BusyHands = equipment.WeaponCurrent.Hands;

			leftHand.DestroyChildren();
			rightHand.DestroyChildren();

			switch (BusyHands)
			{
				case Hands.None:
				{
					break;
				}
				case Hands.Main:
				{
					var prefab = equipment.WeaponCurrent.Main.Item.ItemData.prefab;

					if (prefab != null)
					{
						Instantiate(prefab, rightHand);
					}
					break;
				}
				case Hands.Spare:
				{
					var prefab = equipment.WeaponCurrent.Spare.Item.ItemData.prefab;

					if (prefab != null)
					{
						Instantiate(prefab, leftHand);
					}
					break;
				}
				case Hands.Both:
				{
					var prefab = equipment.WeaponCurrent.Main.Item.ItemData.prefab;

					if (prefab != null)
					{
						Instantiate(prefab, rightHand);
					}
					break;
				}
			}
		}
	}
}