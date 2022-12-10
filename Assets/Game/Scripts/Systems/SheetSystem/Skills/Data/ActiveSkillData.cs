using Sirenix.OdinInspector;
using System.Collections.Generic;

using UnityEngine;

namespace Game.Systems.SheetSystem.Skills
{
	public abstract class ActiveSkillData : SkillData
	{
		[Min(1)]
		public float baseCooldown;
		[Min(0)]
		public int duration;//turns
		public bool isHasLimitationsOnUse = false;
		[ShowIf("isHasLimitationsOnUse")]
		public SkillLimitations limitations;
		[Space]
		[HideLabel]
		public SkillRange range;
		[Space]
		public List<ThrowsType> savingThrows;//or
	}

	public abstract class ActiveTargetSkillData : ActiveSkillData
	{
		[Space]
		[Min(1)]
		public int targetCount = 1;
		public bool isCanTargetSelf = false;

		[Space]
		public bool isCanClampOnTarget = true;

		[Space]
		[LabelText("Is AoE")]
		public bool isAoE = false;
		[ShowIf("isAoE")]
		[HideLabel]
		public SkillAoE AoE;
	}

	[System.Serializable]
	public class SkillLimitations
	{
		public bool isUseLimitOnBattle = false;
		[ShowIf("isUseLimitOnBattle")]
		[Min(1)]
		public int maxCountInBattle = 1;

		public bool isUseLimitToRest = false;
		[ShowIf("isUseLimitToRest")]
		[Min(1)]
		public int maxCountToRest = 1;
	}

	[System.Serializable]
	public class SkillRange
	{
		public RangeType rangeType = RangeType.None;

		[Min(0)]
		[SuffixLabel("m", true)]
		[ShowIf("rangeType", RangeType.Custom)]
		public float range;
	}

	[System.Serializable]
	public class SkillAoE
	{
		[LabelText("AoE Type")]
		public AoEType AoEType = AoEType.Circle;
		[Min(1)]
		[SuffixLabel("m", true)]
		public float range = 1;
	}

	public enum ThrowsType
	{
		Willpower
	}

	public enum RangeType
	{
		None,
		Max,
		Custom,
	}

	public enum AoEType
	{
		Circle,
	}
}