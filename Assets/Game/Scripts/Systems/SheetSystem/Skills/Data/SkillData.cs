using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Systems.SheetSystem.Skills
{
	public abstract class SkillData : ScriptableObject
	{
		[HideLabel]
		public Information information;

		[Min(0)]
		public int requiredLevel;
		
		public string Title => $"{(information.name.IsEmpty() ? name : information.name)}";
	}
}