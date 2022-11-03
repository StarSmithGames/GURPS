using Game.Systems.InventorySystem;

using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game.Systems.SheetSystem.Skills
{
	public sealed class SkillDeck
	{
		public List<SlotSkill> Skills { get; }
		public List<SlotSkill> MemorySkills { get; }

		public SkillDeck(SkillsSettings settings)
		{
			Skills = new List<SlotSkill>();
			MemorySkills = new List<SlotSkill>();

			for (int i = 0; i < settings.skills.Count; i++)
			{
				Skills.Add(new SlotSkill()
				{
					skill = settings.skills[i].Copy(),
				});
			}
		}

		public SlotSkill[] GetSkillSlotsByLevel(int level)
		{
			return Skills.Where((x) => x.skill.level == level).ToArray();
		}

		public List<SkillGroup> GetSkillGroupsByLevel()
		{
			List<SkillGroup> groups = new List<SkillGroup>();

			var levels = Skills.Select((x) => x.skill.level).Distinct().ToArray();

			for (int i = 0; i < levels.Length; i++)
			{
				groups.Add(new SkillGroup()
				{
					level = levels[i],
					skills = GetSkillSlotsByLevel(levels[i]),
				});
			}

			return groups;
		}
	}

	public struct SkillGroup
	{
		public int level;
		public SlotSkill[] skills;
	}

	[System.Serializable]
	public sealed class SkillsSettings
	{
		[ListDrawerSettings(OnBeginListElementGUI = "BeginDrawListElement", OnEndListElementGUI = "EndDrawListElement")]
		[AssetSelector(ExpandAllMenuItems = true, IsUniqueList = true, Paths = "Assets/Game/Resources/Assets/Sheet/Skills")]
		public List<Skill> skills = new List<Skill>();

		private void BeginDrawListElement(int index)
		{
			GUILayout.BeginHorizontal();
		}

		private void EndDrawListElement(int index)
		{
			if (skills[index] != null)
			{
				//if (skills[index].name.StartsWith("Base"))
				//{
				//	GUIStyle style = new GUIStyle();
				//	style.normal.textColor = Color.cyan;
				//	GUILayout.Label("Base", style);
				//}
			}
			GUILayout.EndHorizontal();
		}
	}
}