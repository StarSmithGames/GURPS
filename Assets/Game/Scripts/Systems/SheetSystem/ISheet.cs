using Game.Entities;
using Game.Systems.InventorySystem;
using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
    public interface ISheet
    {
        IStats Stats { get; }
        IInventory Inventory { get; }
    }

    public class EntitySheet : ISheet
    {
        public virtual IStats Stats { get; private set; }
        public virtual IInventory Inventory { get; private set; }

        public EntitySheet(EntityData data)
        {
            Stats = new Stats(data.characterSheet.stats);
            Inventory = new Inventory(data.characterSheet.inventory);
        }
    }

    public class CharacterSheet : EntitySheet
    {
        public virtual IEquipment Equipment { get; private set; }

        public CharacterSheet(EntityData data) : base(data)
        {
            Equipment = new Equipment(data.characterSheet.equipment, Inventory);
        }
    }

    public class NPCSheet : EntitySheet
    {
        public NPCSheet(EntityData data) : base(data) { }
    }


    [System.Serializable]
    public class CharacterSheetSettings
    {
        public Identity identity = Identity.Humanoid;
        public float age = -1;
        [ShowIf("identity", Identity.Humanoid)]
        public Race race = Race.Human;
        public Gender gender = Gender.Male;
        public Aligment aligment = Aligment.TrueNeutral;
        [Space]
        public StatsSettigns stats;
        public InventorySettings inventory;
        [ShowIf("identity", Identity.Humanoid)]
        public EquipmentSettings equipment;
    }

    public enum Identity
    {
        Humanoid,
        Animal,
    }

    public enum Race
    {
        Human = 0,
        Elf = 1,
        Dwarf = 2,
        Dragonborn = 3,

        HalfElf = 10,
        HalfDwarf = 11,
        Halflieng = 12,

        Goblin = 100,
        Hobgoblin = 101,
        Orc = 102,
        Kobold = 103,
        Murloc = 104,
        Minotaur = 105,
        Zombie = 106,
    }

    public enum Gender
    {
        Male,
        Female,
        Neutral,
    }

    //https://rpg.fandom.com/ru/wiki/%D0%9C%D0%B8%D1%80%D0%BE%D0%B2%D0%BE%D0%B7%D0%B7%D1%80%D0%B5%D0%BD%D0%B8%D0%B5
    public enum Aligment
    {
        LawfulGood,
        NeutralGood,
        ChaoticGood,
        LawfulNeutral,
        TrueNeutral,
        ChaoticNeutral,
        LawfulEvil,
        NeutralEvil,
        ChaoticEvil,
    }
}