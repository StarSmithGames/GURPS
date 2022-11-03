using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game.Systems.SheetSystem.Skills
{
	public sealed class SkillDeck
	{
		public RegistratorSlotItem<SkillSlot, Skill> Skills { get; }
		public RegistratorSlotItem<SkillSlot, Skill> MemorySkills { get; }
		//public Registrator<SlotItemBind<SkillSlot, Skill>> s { get; }

		public SkillDeck(SkillsSettings settings)
		{
			Skills = new RegistratorSlotItem<SkillSlot, Skill>();
			MemorySkills = new RegistratorSlotItem<SkillSlot, Skill>();

			for (int i = 0; i < settings.skills.Count; i++)
			{
				Skills.RegistrateBind(new SlotItemBind<SkillSlot, Skill>()
				{
					slot = new SkillSlot(),
					item = settings.skills[i],
				});
			}
		}

		public Skill[] GetSkillsByLevel(int level)
		{
			return Skills.registers.Where((x) => x.item.level == level).Select((y) => y.item).ToArray();
		}

		public List<SkillGroup> GetSkillGroupsByLevel()
		{
			List<SkillGroup> groups = new List<SkillGroup>();

			var levels = Skills.registers.Select((bind) => bind.item.level).Distinct().ToArray();

			for (int i = 0; i < levels.Length; i++)
			{
				groups.Add(new SkillGroup()
				{
					level = levels[i],
					skills = GetSkillsByLevel(levels[i]),
				});
			}

			return groups;
		}
	}

	public struct SkillGroup
	{
		public int level;
		public Skill[] skills;
	}

	[System.Serializable]
	public sealed class SkillSlot : ISlot
	{
		[HideLabel]
		public Skill skill;

		public ISlot Copy()
		{
			return new SkillSlot()
			{
				skill = skill,
			};
		}
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