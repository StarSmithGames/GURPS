using System.Collections.Generic;

namespace Game.Systems.SheetSystem
{
	public abstract class Enchantment { }

	public abstract class ActivationEnchantment : Enchantment, IActivation
	{
		public bool IsActive { get; private set; }

		public virtual void Activate()
		{
			IsActive = true;
		}

		public virtual void Deactivate()
		{
			IsActive = false;
		}
	}

	public abstract class ExecutableEnchantment : Enchantment, IExecutable
	{
		public abstract void Execute();
	}

	public interface IBuff : IActivation
	{
		List<ActivationEnchantment> Enchantments { get; }
	}

	public interface ITrait : IActivation
	{
		List<ActivationEnchantment> Enchantments { get; }
	}

	public interface ITalent : IActivation { }


	#region Enchantment
	public class AddStatModifierEnchantment : ActivationEnchantment
	{
		private AttributeModifier modifier;

		private IStat stat;

		public AddStatModifierEnchantment(IStat stat, int add)
		{
			this.stat = stat;

			modifier = new AddModifier(add);
		}

		public override void Activate()
		{
			base.Activate();
			stat.AddModifier(modifier);
		}

		public override void Deactivate()
		{
			base.Deactivate();
			stat.RemoveModifier(modifier);
		}
	}

	public class AddPercentStatModifierEnchantment : ActivationEnchantment
	{
		private AttributeModifier modifier;

		private IStat stat;

		public AddPercentStatModifierEnchantment(IStat stat, float percent)
		{
			this.stat = stat;

			modifier = new PercentModifier(percent);
		}

		public override void Activate()
		{
			base.Activate();
			stat.AddModifier(modifier);
		}

		public override void Deactivate()
		{
			base.Deactivate();
			stat.RemoveModifier(modifier);
		}
	}


	public class AddStatEnchantment : ExecutableEnchantment
	{
		private IStat stat;
		private float add;

		public AddStatEnchantment(IStat stat, float add)
		{
			this.stat = stat;
			this.add = add;
		}

		public override void Execute()
		{
			stat.CurrentValue += add;
		}
	}
	#endregion

	#region Trait
	public abstract class Trait : ITrait
	{
		public bool IsActive { get; private set; }

		public List<ActivationEnchantment> Enchantments { get; }

		protected ISheet sheet;

		public Trait(ISheet sheet)
		{
			this.sheet = sheet;

			Enchantments = new List<ActivationEnchantment>();
		}

		public void Activate()
		{
			for (int i = 0; i < Enchantments.Count; i++)
			{
				Enchantments[i].Activate();
			}

			IsActive = true;
		}

		public void Deactivate()
		{
			for (int i = 0; i < Enchantments.Count; i++)
			{
				Enchantments[i].Deactivate();
			}

			IsActive = false;
		}
	}

	public sealed class HumanRaceTrait : Trait
	{
		public HumanRaceTrait(ISheet sheet) : base(sheet)
		{
			Enchantments.Add(new AddStatModifierEnchantment(sheet.Stats.Strength, 1));
			Enchantments.Add(new AddStatModifierEnchantment(sheet.Stats.Dexterity, 1));
			Enchantments.Add(new AddStatModifierEnchantment(sheet.Stats.Intelligence, 1));
			Enchantments.Add(new AddStatModifierEnchantment(sheet.Stats.Will, 1));
			Enchantments.Add(new AddStatModifierEnchantment(sheet.Stats.Perception, 1));
		}
	}

	public sealed class DwarfRaceTrait : Trait
	{
		public DwarfRaceTrait(ISheet sheet) : base(sheet)
		{
			Enchantments.Add(new AddStatModifierEnchantment(sheet.Stats.Strength, 2));
			Enchantments.Add(new AddStatModifierEnchantment(sheet.Stats.Intelligence, -1));
			Enchantments.Add(new AddStatModifierEnchantment(sheet.Stats.Perception, -1));
		}
	}

	public sealed class ElfRaceTrait : Trait
	{
		public ElfRaceTrait(ISheet sheet) : base(sheet)
		{
			Enchantments.Add(new AddStatModifierEnchantment(sheet.Stats.Dexterity, 2));
			Enchantments.Add(new AddStatModifierEnchantment(sheet.Stats.Intelligence, 2));
		}
	}
	#endregion
}