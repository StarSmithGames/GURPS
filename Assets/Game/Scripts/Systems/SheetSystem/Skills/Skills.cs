using Game.Entities;
using Sirenix.OdinInspector;
using Game.Systems.InventorySystem;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

namespace Game.Systems.SheetSystem.Skills
{
	public sealed class Skills
	{
		public event UnityAction onRegistratorChanged;
		public event UnityAction onActiveSkillChanged;

		public SkillDeck SkillDeck { get; private set; }

		public bool IsHasActiveSkill => ActiveSkill != null;
		public Skill ActiveSkill { get; private set; }

		public List<ISkill> CurrentPassiveSkills => passiveRegistrator.registers;

		private Registrator<ISkill> passiveRegistrator;

		private ICharacter character;
		private SkillFactory skillFactory;

		public Skills(ICharacter character, SkillFactory skillFactory, SkillsSettings settings)
		{
			this.character = character;
			this.skillFactory = skillFactory;

			SkillDeck = new SkillDeck(settings);

			passiveRegistrator = new Registrator<ISkill>();

			ActivatePassiveSkills();
		}

		public void PrepareSkill(SkillData data)
		{
			ActiveSkill = CreateSkill(data) as Skill;
			//ActiveSkill.BeginProcess();

			onActiveSkillChanged?.Invoke();
		}

		public void CancelPreparation()
		{
			//ActiveSkill?.CancelProcess();
			ActiveSkill = null;

			onActiveSkillChanged?.Invoke();
		}

		private void ActivatePassiveSkills()
		{
			SkillDeck.PassiveSkills.ForEach((skill) =>
			{
				PassiveSkill passiveSkill = CreateSkill(skill) as PassiveSkill;
				passiveRegistrator.Registrate(passiveSkill);
				passiveSkill.Activate();
			});
		}

		private ISkill CreateSkill(SkillData data)
		{
			return skillFactory.Create(data, character);
		}
	}

	public sealed class SkillDeck
	{
		public List<SlotSkill> Skills { get; }
		public List<SlotSkill> MemorySkills { get; }

		public List<PassiveSkillData> PassiveSkills { get; }
		public List<ActiveSkillData> ActiveSkills { get; }

		public SkillDeck(SkillsSettings settings)
		{
			Skills = new List<SlotSkill>();
			MemorySkills = new List<SlotSkill>();

			PassiveSkills = settings.skills.Where((skill) => skill is PassiveSkillData).Cast<PassiveSkillData>().ToList();
			ActiveSkills = settings.skills.Where((skill) => skill is ActiveSkillData).Cast<ActiveSkillData>().ToList();

			ActiveSkills.ForEach((skill) =>
			{
				Skills.Add(new SlotSkill()
				{
					skill = skill,
				});
			});
		}

		public SlotSkill[] GetSkillSlotsByLevel(int level)
		{
			return Skills.Where((x) => x.skill.level == level).ToArray();
		}

		public List<SkillGroup> GetSkillGroupsByLevel()
		{
			List<SkillGroup> groups = new List<SkillGroup>();

			var levels = Skills.Select((x) => x.skill.level).Distinct().OrderBy((y) => y).ToArray();

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
		public List<SkillData> skills = new List<SkillData>();

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