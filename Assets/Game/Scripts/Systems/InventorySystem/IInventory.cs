using System.Collections.Generic;
using UnityEngine.Events;

namespace Game.Systems.InventorySystem
{
    public interface IInventory
    {
        event UnityAction OnInventoryChanged;

        bool IsEmpty { get; }

        List<Item> Items { get; }

        bool Add(Item item);
        bool Remove(Item item);

        void Clear();
    }
}