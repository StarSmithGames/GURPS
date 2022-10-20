namespace Game.Systems.SheetSystem
{
	public abstract class Race : IActivation
	{
		public bool IsActive { get; protected set; }

		protected ISheet sheet;

		public Race(ISheet sheet)
		{
			this.sheet = sheet;
		}

		public static Race GetRace(RaceType type, ISheet sheet)
		{
			switch (type)
			{
				case RaceType.Human:
				{
					return new HumanRace(sheet);
				}
				case RaceType.Dwarf:
				{
					return new DwarfRace(sheet);
				}
				case RaceType.Elf:
				{
					return new ElfRace(sheet);
				}
			}

			return new NoneRace(sheet);
		}

		public virtual void Activate()
		{
			IsActive = true;
		}

		public virtual void Deactivate()
		{
			IsActive = false;
		}
	}

	public class NoneRace : Race
	{
		public NoneRace(ISheet sheet) : base(sheet) { }

		public override void Activate() { }

		public override void Deactivate() { }
	}

	public class HumanRace : Race
	{
		ITrait trait;

		public HumanRace(ISheet sheet) : base(sheet)
		{
			trait = new HumanRaceTrait(sheet);
		}

		public override void Activate()
		{
			sheet.Traits.Registrate(trait);
			base.Activate();
		}

		public override void Deactivate()
		{
			sheet.Traits.UnRegistrate(trait);
			base.Deactivate();
		}
	}

	public class DwarfRace : Race
	{
		ITrait trait;

		public DwarfRace(ISheet sheet) : base(sheet)
		{
			trait = new DwarfRaceTrait(sheet);
		}

		public override void Activate()
		{
			sheet.Traits.Registrate(trait);
			base.Activate();
		}

		public override void Deactivate()
		{
			sheet.Traits.UnRegistrate(trait);
			base.Deactivate();
		}
	}

	public class ElfRace : Race
	{
		ITrait trait;

		public ElfRace(ISheet sheet) : base(sheet)
		{
			trait = new ElfRaceTrait(sheet);
		}

		public override void Activate()
		{
			sheet.Traits.Registrate(trait);
			base.Activate();
		}

		public override void Deactivate()
		{
			sheet.Traits.UnRegistrate(trait);
			base.Deactivate();
		}
	}

	public enum RaceType
	{
		None = -1,

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
}