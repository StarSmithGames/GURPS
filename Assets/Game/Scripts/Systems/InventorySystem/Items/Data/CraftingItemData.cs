using UnityEngine;

namespace Game.Systems.InventorySystem
{
	[CreateAssetMenu(menuName = "Game/Inventory/Items/Craft", fileName = "Item")]
	public class CraftingItemData : ItemData
	{
        public Crafting crafting = Crafting.CraftingMaterial;
    }
    public enum Crafting
    {
        CraftingMaterial,
        BlacksmithPlan,
        JewelerDesign,
        PageOfTraining,
        Dye,
        Gem,
    }
}