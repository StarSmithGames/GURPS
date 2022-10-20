namespace Game.Systems.SheetSystem
{
	public interface IStat : IAttribute { }
	public interface IStatBar : IStat, IBar { }

	public abstract class Stat : Attribute, IStat
	{
		public override string LocalizationKey => $"{base.LocalizationKey}stats.";

		protected Stat(float currentValue) : base(currentValue) { }
	}

	public abstract class StatBar : AttributeBar, IStatBar
	{
		public override string LocalizationKey => $"{base.LocalizationKey}stats.";

		public StatBar(float value, float min, float max) : base(value, min, max) { }
	}

	#region IStat
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
	#endregion

	#region IStatBar
	public class HitPointsStat : StatBar
	{
		public HitPointsStat(float currentValue, float minValue, float maxValue) : base(currentValue, minValue, maxValue) { }
	}
	public class FatiguePointsStat : StatBar
	{
		public FatiguePointsStat(float currentValue, float minValue, float maxValue) : base(currentValue, minValue, maxValue) { }
	}
	public class MoveStat : StatBar
	{
		//yard to metre
		public override float CurrentValue { get => base.CurrentValue * 0.9144f; set => base.CurrentValue = value; }

		public MoveStat(float currentValue, float minValue, float maxValue) : base(currentValue, minValue, maxValue) { }
	}
	public class SpeedStat : StatBar
	{
		public SpeedStat(float currentValue, float minValue, float maxValue) : base(currentValue, minValue, maxValue) { }
	}
	public class WillStat : StatBar
	{
		public WillStat(float currentValue, float minValue, float maxValue) : base(currentValue, minValue, maxValue) { }
	}
	public class PerceptionStat : StatBar
	{
		public PerceptionStat(float currentValue, float minValue, float maxValue) : base(currentValue, minValue, maxValue) { }
	}

	public class ActionPointsStat : StatBar
	{
		public ActionPointsStat(float currentValue, float minValue, float maxValue) : base(currentValue, minValue, maxValue) { }
	}


	public class LiftStat : StatBar
	{
		public LiftStat(float currentValue, float minValue, float maxValue) : base(currentValue, minValue, maxValue) { }
	}
	#endregion
}