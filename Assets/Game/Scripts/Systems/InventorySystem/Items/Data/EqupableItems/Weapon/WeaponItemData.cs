using Game.Entities;

using UnityEngine;

namespace Game.Systems.InventorySystem
{
    public abstract class WeaponItemData : EquippableItemData
    {
        public DamageSystem.WeaponDamage weaponDamage;

        public OutfitSlotOffset sheathForRightHandTransfrom;
        public OutfitSlotOffset sheathForLeftHandTransfrom;
        public OutfitSlotOffset rightHandTransfrom;
        public OutfitSlotOffset leftHandTransfrom;
    }
}