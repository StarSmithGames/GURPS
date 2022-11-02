using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

using UnityEditor.Experimental.GraphView;

using UnityEngine;

namespace Game.Systems.SheetSystem.Skills
{
	public sealed class Skills : Registrator<Skill>
	{
		public Skills(SkillsSettings settings)
		{
			for (int i = 0; i < settings.skills.Count; i++)
			{
				Registrate(settings.skills[i].Copy());
			}
		}

		public Skill[] GetSkillsByLevel(int level)
		{
			return registers.Where((x) => x.level == level).ToArray();
		}

		public List<SkillGroup> GetSkillGroupsByLevel()
		{
			List<SkillGroup> groups = new List<SkillGroup>();

			var levels = registers.Select((skill) => skill.level).Distinct().ToArray();

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