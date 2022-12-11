using Game.Systems.BattleSystem.TargetSystem;

using Sirenix.OdinInspector;
using System.Collections.Generic;

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
		public TargetRange range;
	}

	public abstract class ActiveTargetSkillData : ActiveSkillData
	{
		[BoxGroup("Path")]
		[HideLabel]
		public TargetPath path;

		[BoxGroup("AoE")]
		[LabelText("Is AoE")]
		public bool isAoE = false;
		[BoxGroup("AoE")]
		[ShowIf("isAoE")]
		[HideLabel]
		public TargetAoE AoE;

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

	public enum ThrowsType
	{
		Willpower
	}
}