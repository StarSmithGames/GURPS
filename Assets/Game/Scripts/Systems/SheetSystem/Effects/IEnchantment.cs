using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.SheetSystem
{
	public interface IEnchantment : IActivation { }

	public interface IEffect : IActivation { }

	public interface IBuff : IActivation
	{
		List<IEnchantment> Enchantments { get; }
	}

	public interface ITrait : IActivation { }


	#region Enchantment
	public class AddStatEnchantment : IEnchantment
	{
		public bool IsActive { get; private set; }

		private AttributeModifier modifier;

		private IStat stat;

		public AddStatEnchantment(IStat stat, int add)
		{
			this.stat = stat;

			modifier = new AddModifier(add);
		}

		public void Activate()
		{
			stat.AddModifier(modifier);
		}

		public void Deactivate()
		{
			stat.RemoveModifier(modifier);
		}
	}

	public class AddPercentStatEnchantment : IEnchantment
	{
		public bool IsActive { get; private set; }

		private AttributeModifier modifier;

		private IStat stat;

		public AddPercentStatEnchantment(IStat stat, float percent)
		{
			this.stat = stat;

			modifier = new PercentModifier(percent);
		}

		public void Activate()
		{
			stat.AddModifier(modifier);
		}

		public void Deactivate()
		{
			stat.RemoveModifier(modifier);
		}
	}
	#endregion



	#region Trait
	public abstract class Trait : ITrait
	{
		public bool IsActive { get; private set; }

		public List<IEnchantment> Enchantments { get; }

		protected ISheet sheet;

		public Trait(ISheet sheet)
		{
			this.sheet = sheet;

			Enchantments = new List<IEnchantment>();
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
			Enchantments.Add(new AddStatEnchantment(sheet.Stats.Strength, 1));
			Enchantments.Add(new AddStatEnchantment(sheet.Stats.Dexterity, 1));
			Enchantments.Add(new AddStatEnchantment(sheet.Stats.Intelligence, 1));
			Enchantments.Add(new AddStatEnchantment(sheet.Stats.Will, 1));
			Enchantments.Add(new AddStatEnchantment(sheet.Stats.Perception, 1));
		}
	}

	public sealed class DwarfRaceTrait : Trait
	{
		public DwarfRaceTrait(ISheet sheet) : base(sheet)
		{
			Enchantments.Add(new AddStatEnchantment(sheet.Stats.Strength, 2));
			Enchantments.Add(new AddStatEnchantment(sheet.Stats.Intelligence, -1));
			Enchantments.Add(new AddStatEnchantment(sheet.Stats.Perception, -1));
		}
	}

	public sealed class ElfRaceTrait : Trait
	{
		public ElfRaceTrait(ISheet sheet) : base(sheet)
		{
			Enchantments.Add(new AddStatEnchantment(sheet.Stats.Dexterity, 2));
			Enchantments.Add(new AddStatEnchantment(sheet.Stats.Intelligence, 2));
		}
	}
	#endregion
}