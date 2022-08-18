using Game.Entities;
using Game.Systems.DialogueSystem;
using Game.Systems.InventorySystem;
using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
    public interface ISheet
    {
        EntityInformation Information { get; }
        IStats Stats { get; }
        ICharacteristics Characteristics { get; }
        //ITalents
        //abilities
        //traits 
        IConditions Conditions { get; }
        IInventory Inventory { get; }

        SheetSettings Settings { get; }
    }

    public abstract class EntitySheet : ISheet
    {
        public virtual EntityInformation Information { get; protected set; }
        public virtual IStats Stats { get; private set; }
		public virtual ICharacteristics Characteristics { get; private set; }
        public virtual IConditions Conditions { get; private set; }
        public virtual IInventory Inventory { get; private set; }

		public SheetSettings Settings { get; private set; }
		public ActorSettings ActorSettings { get; }

		public EntitySheet(EntityInformation information, SheetSettings sheetSettings)
        {
            Settings = sheetSettings;

            Information = information;
            Stats = new Stats(Settings.stats);
            Characteristics = new Characteristics(Settings.characteristics);
            Conditions = new Conditions();
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

	public class ModelSheet : EntitySheet
	{
		public ModelSheet(ModelData data) : base(data.information, data.sheet) { }
	}

	public class ContainerSheet : ModelSheet
    {
		public ContainerSheet(ContainerData data) : base(data) { }
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
        //[HideIf("IsLifeless")]
        //public Aligment aligment = Aligment.TrueNeutral;
        [Space]
        public bool isImmortal = false;
        public StatsSettigns stats;
        public CharacteristicsSettings characteristics;
        public InventorySettings inventory;
        [ShowIf("@IsHumanoid && !IsLifeless")]
        public EquipmentSettings equipment;
        public ActorSettings actor;        

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
}