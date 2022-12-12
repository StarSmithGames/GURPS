using Game.Entities;
using Game.Systems.DialogueSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem.Abilities;
using Game.Systems.SheetSystem.Effects;
using Game.Systems.SheetSystem.Skills;
using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

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

		Effects.Effects Effects { get; }

		Conditions Conditions { get; }

        Traits Traits { get; }
        Talents Talents { get; }
        //racial abilities
        //Personality Traits
        //Phobias?
        //Ancestry?

        SheetSettings Settings { get; }

        void Restore();
	}

    public abstract class EntitySheet : ISheet
    {
        public virtual EntityInformation Information { get; protected set; }

        public virtual Race Race { get; private set; }
        public virtual Stats Stats { get; private set; }
		public virtual Characteristics Characteristics { get; private set; }

        public virtual Inventory Inventory { get; private set; }

		public virtual Effects.Effects Effects { get; protected set; }

		public virtual Conditions Conditions { get; private set; }

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
            
            Inventory = new Inventory(Settings.inventory, this);

			Conditions = new Conditions();
            Traits = new Traits();
            Talents = new Talents();

            Race.Activate();
		}

        public virtual void Restore()
        {
			Stats.HitPoints.Restore();
			Stats.FatiguePoints.Restore();
			Stats.Move.Restore();
			Stats.Speed.Restore();
			Stats.Will.Restore();
			Stats.Perception.Restore();
			Stats.ActionPoints.Restore();
		}
    }

	public abstract class ModelSheet : EntitySheet
	{
		public ModelSheet(ModelData data, EffectFactory effectFactory) : base(data.information, data.sheet)
		{
			Effects = new Effects.Effects(this, effectFactory);
		}
	}

	public sealed class CharacterSheet : ModelSheet
	{
		public SkillDeck SkillDeck { get; private set; }

		public Equipment Equipment { get; private set; }
		public ActionBar ActionBar { get; private set; }

		public CharacterSheet(CharacterData data, EffectFactory effectFactory) : base(data, effectFactory)
        {
			SkillDeck = new SkillDeck(data.sheet.skills);
			Equipment = new Equipment(data.sheet.equipment, this);
			ActionBar = new ActionBar(this);
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

        public class Factory : PlaceholderFactory<CharacterData, CharacterSheet> { }
    }

	public sealed class ContainerSheet : ModelSheet
    {
		public ContainerSheet(ContainerData data, EffectFactory effectFactory) : base(data, effectFactory) { }

		public class Factory : PlaceholderFactory<ContainerData, ContainerSheet> { }
	}


	public class SheetFactory : PlaceholderFactory<ModelData, ISheet> { }

	public class CustomSheetFactory : IFactory<ModelData, ISheet>
	{
        private CharacterSheet.Factory characterSheetFactory;
        private ContainerSheet.Factory containerSheetFactory;

		public CustomSheetFactory(
			CharacterSheet.Factory characterSheetFactory,
            ContainerSheet.Factory containerSheetFactory)
        {
            this.characterSheetFactory = characterSheetFactory;
            this.containerSheetFactory = containerSheetFactory;
		}

		public ISheet Create(ModelData data)
		{
            if(data == null)
            {
                throw new System.NullReferenceException();
            }

            if(data is CharacterData characterData)
            {
                return characterSheetFactory.Create(characterData);
			}
            else if(data is ContainerData containerData)
            {
                return containerSheetFactory.Create(containerData);
			}

			throw new System.NotImplementedException();
		}
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
        [TabGroup("GroupB", "Skills")]
        public SkillsSettings skills;
        
        [TabGroup("GroupC", "Abilities")]
        public AbilitiesSettings abilities;

        [TabGroup("GroupA", "Custom")]
        public bool isImmortal = false;
	}
}