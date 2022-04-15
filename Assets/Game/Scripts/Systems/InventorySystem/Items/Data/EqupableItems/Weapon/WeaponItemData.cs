using Game.Entities;

using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.InventorySystem
{
    public abstract class WeaponItemData : EquippableItemData
    {
        public DamageSystem.WeaponDamage weaponDamage;
        [SuffixLabel("m", true)]
        [Range(0.8f, 26f)]
        public float weaponRange = 0.8f;
        [Space]
        public Transform3 sheathForRightHandTransfrom;
        public Transform3 sheathForLeftHandTransfrom;
        public Transform3 rightHandTransfrom;
        public Transform3 leftHandTransfrom;
    }
}