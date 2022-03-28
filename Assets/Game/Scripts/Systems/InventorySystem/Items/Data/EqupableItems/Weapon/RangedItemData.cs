using UnityEngine;

namespace Game.Systems.InventorySystem
{
	[CreateAssetMenu(menuName = "Game/Inventory/Items/Equipable/Weapon/Ranged", fileName = "Item")]
	public class RangedItemData : WeaponItemData
	{
		public RangedType rangedType = RangedType.Bow;
	}
	public enum RangedType
	{
		Bow,
		CrossBow,
		MagicWand,
	}
}