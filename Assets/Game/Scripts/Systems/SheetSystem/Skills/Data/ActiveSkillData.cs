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
		public Limitations limitations;

		public List<ThrowsType> savingThrows;//or
	}

	[System.Serializable]
	public class Limitations
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

	public enum ThrowsType
	{
		Willpower
	}
}