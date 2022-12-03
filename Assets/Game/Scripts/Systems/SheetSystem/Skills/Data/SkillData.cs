using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.SheetSystem.Skills
{
	public abstract class SkillData : ScriptableObject, IAction
	{
		[HideLabel]
		public Information information;

		[Range(0, 9)]
		public int level;

		public string Title => $"{(information.name.IsEmpty() ? name : information.name)}";
	}
}