using Game.Entities;
using Game.Systems.DialogueSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem.Abilities;

using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
    //Deaty & Domains
    public interface ISheet
    {
        EntityInformation Information { get; }

        IStats Stats { get; }
        ICharacteristics Characteristics { get; }

        IInventory Inventory { get; }

        Conditions Conditions { get; }
        Abilities.Abilities Abilities { get; }
        Skills Skills { get; }
        Traits Traits { get; }
        Talents Talents { get; }
        //racial abilities
        //Personality Traits
        //Phobias?
        //Ancestry?

        SheetSettings Settings { get; }
    }

    public abstract class EntitySheet : ISheet
    {
        public virtual EntityInformation Information { get; protected set; }

        public virtual IStats Stats { get; private set; }
		public virtual ICharacteristics Characteristics { get; private set; }

        public virtual IInventory Inventory { get; private set; }

        public virtual Conditions Conditions { get; private set; }
        public virtual Abilities.Abilities Abilities { get; private set; }
        public virtual Skills Skills { get; private set; }
        public virtual Traits Traits { get; private set; }
        public virtual Talents Talents { get; private set; }

        public SheetSettings Settings { get; private set; }
		public ActorSettings ActorSettings { get; private set; }

		public EntitySheet(EntityInformation information, SheetSettings sheetSettings)
        {
            Settings = sheetSettings;

            Information = information;
            
            Stats = new Stats(Settings.stats);
            Characteristics = new Characteristics(Settings.characteristics);
            
            Inventory = new Inventory(Settings.inventory);

            Conditions = new Conditions();
            //Abilities = new Abilities.Abilities(Settings.abilities);
            Skills = new Skills();
            Traits = new Traits();
            Talents = new Talents();
        }
    }

    public sealed class CharacterSheet : EntitySheet
    {
        public IEquipment Equipment { get; private set; }

        public CharacterSheet(CharacterData data) : base(data.information, data.sheet)
        {
            Equipment = new Equipment(data.sheet.equipment, Inventory);
        }

        public Data GetData()
		{
            return new Data
            {

            };
		}

        public class Data
		{

		}
    }

	public class ModelSheet : EntitySheet
	{
		public ModelSheet(ModelData data) : base(data.information, data.sheet) { }
	}

	public sealed class ContainerSheet : ModelSheet
    {
		public ContainerSheet(ContainerData data) : base(data) { }
	}


    [HideLabel]
    [System.Serializable]
    public sealed class SheetSettings
    {
        //Birthday
        [TabGroup("GroupA", "Info")]
        public SheetInfo info;
        [Space]
        [TabGroup("GroupA", "Stats")]
        public StatsSettigns stats;
        [TabGroup("GroupA", "Characteristics")]
        public CharacteristicsSettings characteristics;
        [TabGroup("GroupA", "Actor")]
        public ActorSettings actor;

        [TabGroup("GroupB", "Inventory")]
        public InventorySettings inventory;
        [TabGroup("GroupB", "Equipment")]
        public EquipmentSettings equipment;

        [TabGroup("GroupC", "Abilities")]
        public AbilitiesSettings abilities;

        [TabGroup("GroupA", "Custom")]
        public bool isImmortal = false;
    }

    [HideLabel]
    [System.Serializable]
    public sealed class SheetInfo
	{
        public Identity identity = Identity.Humanoid;

        [ShowIf("@IsHumanoid && !IsLifeless")]
        public Race race = Race.Human;
        [HideIf("IsLifeless")]
        public Gender gender = Gender.Male;
        [HideIf("IsLifeless")]
        [Range(0, 100)]
        public float age = -1;

        public bool IsHumanoid => identity == Identity.Humanoid;
        public bool IsLifeless => identity == Identity.Lifeless;
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
        Neuter,
        Neutral,
	}
}