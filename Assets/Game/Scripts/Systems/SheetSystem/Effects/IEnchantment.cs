using Sirenix.OdinInspector;

using System.Collections.Generic;

namespace Game.Systems.SheetSystem
{
	public abstract class Enchantment : IActivation
	{
		public bool IsActive { get; private set; }

		public virtual bool IsReversible => true;

		public virtual void Activate()
		{
			IsActive = true;
		}

		public virtual void Deactivate()
		{
			IsActive = false;
		}
	}

	public interface IBuff : IActivation
	{
		List<Enchantment> Enchantments { get; }
	}

	public interface ITrait : IActivation
	{
		List<Enchantment> Enchantments { get; }
	}

	public interface ITalent : IActivation { }


	[System.Serializable]
	public abstract class EnchantmentType
	{
		public abstract Enchantment GetEnchantment(object obj);
	}

	[System.Serializable]
	public sealed class AddHealthPoints : EnchantmentType
	{
		public float add;

		public override Enchantment GetEnchantment(object obj)
		{
			var sheet = (ISheet)obj;
			return new AddStatEnchantment(sheet.Stats.HitPoints, add);
		}
	}

	[System.Serializable]
	public sealed class AddHealthPointsModifier : EnchantmentType
	{
		public bool isPercent = false;
		[HideIf("isPercent")]
		public float add;
		[ShowIf("isPercent")]
		[SuffixLabel("%", overlay: true)]
		public float addPercent;

		public override Enchantment GetEnchantment(object obj)
		{
			var sheet = (ISheet)obj;
			if (isPercent)
			{
				return new AddPercentStatModifierEnchantment(sheet.Stats.HitPoints, addPercent);
			}
			else
			{
				return new AddStatModifierEnchantment(sheet.Stats.HitPoints, add);
			}
		}
	}


	#region Enchantment
	public class AddStatModifierEnchantment : Enchantment
	{
		private AttributeModifier modifier;

		private IStat stat;

		public AddStatModifierEnchantment(IStat stat, float add)
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

	public class AddPercentStatModifierEnchantment : Enchantment
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


	public class AddStatEnchantment : Enchantment
	{
		public override bool IsReversible => false;

		private IStat stat;
		private float add;

		public AddStatEnchantment(IStat stat, float add)
		{
			this.stat = stat;
			this.add = add;
		}

		public override void Activate()
		{
			stat.CurrentValue += add;
		}

		public override void Deactivate() { }
	}
	#endregion

	#region Trait
	public abstract class Trait : ITrait
	{
		public bool IsActive { get; private set; }

		public List<Enchantment> Enchantments { get; }

		protected ISheet sheet;

		public Trait(ISheet sheet)
		{
			this.sheet = sheet;

			Enchantments = new List<Enchantment>();
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