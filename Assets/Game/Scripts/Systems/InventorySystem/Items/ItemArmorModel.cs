using UnityEngine;

namespace Game.Systems.InventorySystem
{
    public class ItemArmorModel : ItemModel
    {
        public SkinnedMeshRenderer Renderer => renderer;
        [SerializeField] private SkinnedMeshRenderer renderer;
    }
}