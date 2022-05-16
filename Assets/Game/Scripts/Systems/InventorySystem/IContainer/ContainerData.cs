using Game.Entities;
using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Systems.InventorySystem
{
    [CreateAssetMenu(fileName = "ContainerData", menuName = "Game/Inventory/Container")]
    public class ContainerData : EntityData
    {
        public ContainerInformation information;
    }
}