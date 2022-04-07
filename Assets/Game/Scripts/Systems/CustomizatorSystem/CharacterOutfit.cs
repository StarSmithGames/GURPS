using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using UnityEngine;

using Zenject;

namespace Game.Entities
{
	public class CharacterOutfit : MonoBehaviour
	{
		public Hands BusyHands { get; private set; }

		[SerializeField] private Transform leftHand;
		[SerializeField] private Transform rightHand;

		private IEquipment equipment;

		[Inject]
		private void Construct(Entity entity)
		{
			equipment = ((entity as Character).Sheet as CharacterSheet).Equipment;
		}

		private void OnDestroy()
		{
			if (equipment != null)
			{
				equipment.OnEquipmentChanged -= OnEquipmentChanged;
			}
		}

		private void Start()
		{
			equipment.OnEquipmentChanged += OnEquipmentChanged;

			OnEquipmentChanged();
		}

		private void OnEquipmentChanged()
		{
			BusyHands = equipment.WeaponCurrent.Hands;
		}
	}
}