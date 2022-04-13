using Sirenix.OdinInspector;

using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.InventorySystem
{
    public class ItemModel : MonoBehaviour
    {
        public Item Item => item;
        [SerializeField] protected Item item;
    }
}