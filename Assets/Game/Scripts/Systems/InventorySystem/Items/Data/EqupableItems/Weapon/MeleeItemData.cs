using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.InventorySystem
{
    [CreateAssetMenu(menuName = "Game/Inventory/Items/Equipable/Weapon/Melee", fileName = "Item")]
    public class MeleeItemData : WeaponItemData
    {
        public MelleType melleType = MelleType.OneHanded;
        [ShowIf("melleType", MelleType.OffHanded)]
        public OffHandedType offHandType = OffHandedType.Shield;
        [ShowIf("melleType", MelleType.OneHanded)]
        public OneHandedType oneHandedType = OneHandedType.Sword;
        [ShowIf("melleType", MelleType.TwoHanded)]
        public TwoHandedType twoHandedType = TwoHandedType.Sword;
    }
    public enum MelleType
	{
        OffHanded,
        OneHanded,
        TwoHanded,
	}

    public enum OffHandedType
    {
        Shield,
    }
    public enum OneHandedType
    {
        Axe,
        Dagger,
        Mace,
        Spear,
        Sword,
    }
    public enum TwoHandedType
    {
        Axe,
        Mace,
        Polearm,
        Sword,
        Staff,
    }
}