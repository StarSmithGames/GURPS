namespace Game.Systems.InventorySystem
{
    public abstract class WeaponItemData : EquippableItemData
    {
        public DamageSystem.DamageComposite damage;
        public DamageSystem.Resistances resistances;
    }
}