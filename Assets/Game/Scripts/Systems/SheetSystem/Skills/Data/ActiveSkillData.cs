using Sirenix.OdinInspector;
using System.Collections.Generic;

using UnityEditor.UIElements;

using UnityEngine;

namespace Game.Systems.SheetSystem.Skills
{
	public abstract class ActiveSkillData : SkillData
	{
		[Space]
		public List<ThrowsType> savingThrows;//or
		[HideLabel]
		[BoxGroup("Limitations")]
		public SkillLimitations limitations;
		[HideLabel]
		[BoxGroup("Range")]
		public SkillRange range;
	}

	public abstract class ActiveTargetSkillData : ActiveSkillData
	{
		[BoxGroup("Path")]
		[HideLabel]
		public SkillPath path;

		[BoxGroup("AoE")]
		[LabelText("Is AoE")]
		public bool isAoE = false;
		[BoxGroup("AoE")]
		[ShowIf("isAoE")]
		[HideLabel]
		public SkillAoE AoE;

		[Min(1)]
		public int targetCount = 1;
		public bool isCanTargetSelf = false;
		public bool isCanClampOnTarget = true;
		//calculate collider
	}

	[System.Serializable]
	public class SkillLimitations
	{
		[Min(1)]
		public float baseCooldown;
		[Min(0)]
		public int duration;//turns
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
	public class SkillPath
	{
		public bool drawPath = true;
		public PathType pathType = PathType.Line;
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

	public enum PathType
	{
		Line,
		Ballistic,
		Custom
	}
}