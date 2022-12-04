using Game.Entities;
using UnityEngine;

namespace Game.Systems.InventorySystem
{
    [CreateAssetMenu(fileName = "ContainerData", menuName = "Game/Inventory/Container")]
    public class ContainerData : ModelData
    {
        public bool isLocked = false;
    }
}