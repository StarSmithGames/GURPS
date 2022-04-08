using UnityEngine.Events;
using UnityEngine;

namespace Game.Systems.SheetSystem
{
    public interface IStat : IAttribute, IModifiable
    {
		event UnityAction onStatChanged;

		float CurrentValue { get; set; }
    }

	public interface IStatBar : IStat
	{
		float MaxBaseValue { get; }
		float MaxValue { get; }

		float PercentValue { get; }
	}

	public abstract class Stat : AttributeModifiable, IStat
	{
		public event UnityAction onStatChanged;

		public virtual float CurrentValue
		{
			get => currentValue;
			set
			{
				currentValue = value;

				ValueChanged();
			}
		}
		protected float currentValue;

		public Stat(float currentValue) : base()
		{
			this.currentValue = currentValue;
		}

		protected override void ValueChanged()
		{
			base.ValueChanged();

			onStatChanged?.Invoke();
		}

		public override string ToString()
		{
			return CurrentValue.ToString();
		}
	}

	public abstract class StatBar : AttributeModifiable, IStatBar
	{
		public event UnityAction onStatChanged;

		public virtual float CurrentValue
		{
			get => currentValue;
			set
			{
				currentValue = Mathf.Clamp(value, 0, MaxValue);

				ValueChanged();
			}
		}
		protected float currentValue;

		public virtual float MaxBaseValue
		{
			get => maxBaseValue;
			set
			{
				maxBaseValue = value;

				ValueChanged();
			}
		}
		protected float maxBaseValue;

		public float MaxValue => MaxBaseValue + ModifyValue;

		public virtual float PercentValue => currentValue / MaxValue;

		public StatBar(float currentValue, float maxBaseValue) : base()
		{
			this.currentValue = currentValue;
			this.maxBaseValue = maxBaseValue;
		}

		protected override void ValueChanged()
		{
			base.ValueChanged();

			onStatChanged?.Invoke();
		}

		public override string ToString()
		{
			return CurrentValue + "/" + MaxValue;
		}
	}

	public class LevelStat : Stat
	{
		public LevelStat(float currentValue) : base(currentValue) { }
	}

	public class StrengthStat : Stat
	{
		public StrengthStat(float currentValue) : base(currentValue) { }
	}
	public class DexterityStat : Stat
	{
		public DexterityStat(float currentValue) : base(currentValue) { }
	}
	public class IntelligenceStat : Stat
	{
		public IntelligenceStat(float currentValue) : base(currentValue) { }
	}
	public class HealthStat : Stat
	{
		public HealthStat(float currentValue) : base(currentValue) { }
	}


	public class HitPointsStat : StatBar
	{
		public HitPointsStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }
	}
	public class FatiguePointsStat : StatBar
	{
		public FatiguePointsStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }
	}
	public class MoveStat : StatBar
	{
		//yard to metre
		public override float CurrentValue { get => base.CurrentValue * 0.9144f; set => base.CurrentValue = value; }

		public MoveStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }
	}
	public class SpeedStat : StatBar
	{
		public SpeedStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }
	}
	public class WillStat : StatBar
	{
		public WillStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }
	}
	public class PerceptionStat : StatBar
	{
		public PerceptionStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }
	}

	public class ActionPointsStat : StatBar
	{
		public ActionPointsStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }
	}


	public class LiftStat : StatBar
	{
		public LiftStat(float currentValue, float maxBaseValue) : base(currentValue, maxBaseValue) { }
	}
}