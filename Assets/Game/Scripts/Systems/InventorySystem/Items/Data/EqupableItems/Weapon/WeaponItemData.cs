using Game.Entities;

using UnityEngine;

namespace Game.Systems.InventorySystem
{
    public abstract class WeaponItemData : EquippableItemData
    {
        public DamageSystem.WeaponDamage weaponDamage;

        public Transform3 sheathForRightHandTransfrom;
        public Transform3 sheathForLeftHandTransfrom;
        public Transform3 rightHandTransfrom;
        public Transform3 leftHandTransfrom;
    }
}