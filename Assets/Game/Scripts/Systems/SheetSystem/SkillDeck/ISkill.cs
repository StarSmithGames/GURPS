using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Systems.SheetSystem.Skills
{
	public abstract class Skill : MonoBehaviour, ISlotable, ICopyable<Skill>
	{
		[HideLabel]
		public Information information;

		[Range(0, 9)]
		public int level;

		public abstract Skill Copy();

		public string Title => $"{(information.name.IsEmpty() ? name : information.name)}";
	}

	public enum SkillType
	{
		Action,
		BonusAction,
	}
}