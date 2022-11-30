using Game.Entities;
using Game.Systems.DialogueSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem.Abilities;
using Game.Systems.SheetSystem.Skills;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using System;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
    //Deaty & Domains
    public interface ISheet
    {
        EntityInformation Information { get; }

        Race Race { get; }
        Stats Stats { get; }
        Characteristics Characteristics { get; }

        Inventory Inventory { get; }
        ActionBar ActionBar { get; }

        Conditions Conditions { get; }

        SkillDeck Skills { get; }
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

        public virtual Race Race { get; private set; }
        public virtual Stats Stats { get; private set; }
		public virtual Characteristics Characteristics { get; private set; }

        public virtual Inventory Inventory { get; private set; }
        public virtual ActionBar ActionBar { get; private set; }

        public virtual Conditions Conditions { get; private set; }

        public virtual SkillDeck Skills { get; private set; }
        public virtual Traits Traits { get; private set; }
        public virtual Talents Talents { get; private set; }

        public SheetSettings Settings { get; private set; }
		public ActorSettings ActorSettings { get; private set; }

		public EntitySheet(EntityInformation information, SheetSettings sheetSettings)
        {
            Settings = sheetSettings;

            Information = information;

            Race = Race.GetRace(RaceType.None, this);
            Stats = new Stats(Settings.stats);
            Characteristics = new Characteristics(Settings.characteristics);
            
            Inventory = new Inventory(Settings.inventory);
            ActionBar = new ActionBar(Settings.actionBar, this);

            Conditions = new Conditions();
            Skills = new SkillDeck(Settings.skills);
            Traits = new Traits();
            Talents = new Talents();

            Race.Activate();
        }
    }

    public sealed class CharacterSheet : EntitySheet
    {
		public Equipment Equipment { get; private set; }

		public CharacterSheet(CharacterData data) : base(data.information, data.sheet)
        {
			Equipment = new Equipment(data.sheet.equipment, this);
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
		[TabGroup("GroupB", "ActionBar")]
        public ActionBarSettings actionBar;
        [TabGroup("GroupB", "Skills")]
        public SkillsSettings skills;

        
        [TabGroup("GroupC", "Abilities")]
        public AbilitiesSettings abilities;

        [TabGroup("GroupA", "Custom")]
        public bool isImmortal = false;
	}
}