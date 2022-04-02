using Game.Entities;
using Game.Systems.InventorySystem;
using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
    public interface ISheet
    {
        IInformation Information { get; }
        IStats Stats { get; }
        IInventory Inventory { get; }

        SheetSettings Settings { get; }
    }

    public abstract class EntitySheet : ISheet
    {
        public virtual IInformation Information { get; protected set; }
        public virtual IStats Stats { get; private set; }
        public virtual IInventory Inventory { get; private set; }

		public SheetSettings Settings { get; private set; }

		public EntitySheet(IInformation information, SheetSettings sheetSettings)
        {
            Settings = sheetSettings;

            Information = information;
            Stats = new Stats(Settings.stats);
            Inventory = new Inventory(Settings.inventory);
        }
    }

    public class CharacterSheet : EntitySheet
    {
        public virtual IEquipment Equipment { get; private set; }

        public CharacterSheet(CharacterData data) : base(data.information, data.sheet)
        {
            Equipment = new Equipment(data.sheet.equipment, Inventory);
        }
    }

    public class NPCSheet : EntitySheet
    {
        public NPCSheet(NPCData data) : base(data.information, data.sheet) { }
	}

	public class ContainerSheet : EntitySheet
	{
		public ContainerSheet(ContainerData data) : base(data.information, data.sheet) { }
	}



	[System.Serializable]
    public class SheetSettings
    {
        public Identity identity = Identity.Humanoid;
        [HideIf("IsLifeless")]
        public float age = -1;
        [ShowIf("@IsHumanoid && !IsLifeless")]
        public Race race = Race.Human;
        [HideIf("IsLifeless")]
        public Gender gender = Gender.Male;
        [HideIf("IsLifeless")]
        public Aligment aligment = Aligment.TrueNeutral;
        [Space]
        public bool isImmortal = false;
        public StatsSettigns stats;
        public InventorySettings inventory;
        [ShowIf("@IsHumanoid && !IsLifeless")]
        public EquipmentSettings equipment;


        private bool IsHumanoid => identity == Identity.Humanoid;
        private bool IsLifeless => identity == Identity.Lifeless;
    }

    public enum Identity
    {
        Humanoid    = 0,
        Animal      = 10,
        Lifeless    = 100,
    }

    public enum Race
    {
        Human       = 0,
        Elf         = 1,
        Dwarf       = 2,
        Dragonborn  = 3,

        HalfElf     = 10,
        HalfDwarf   = 11,
        Halflieng   = 12,

        Goblin      = 100,
        Hobgoblin   = 101,
        Orc         = 102,
        Kobold      = 103,
        Murloc      = 104,
        Minotaur    = 105,
        Zombie      = 106,
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