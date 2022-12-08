using Game.Entities;
using Sirenix.OdinInspector;
using Game.Systems.InventorySystem;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using Game.Entities.Models;
using UnityEngine.Assertions;

namespace Game.Systems.SheetSystem.Skills
{
	public sealed class Skills
	{
		public event UnityAction onRegistratorChanged;
		public event UnityAction onActiveSkillChanged;

		public bool IsHasActiveSkill => ActiveSkill != null;
		public ActiveSkill ActiveSkill { get; private set; }

		public List<ISkill> CurrentActiveSkills => activeRegistrator.registers;
		public List<ISkill> CurrentPassiveSkills => passiveRegistrator.registers;

		private Registrator<ISkill> activeRegistrator;
		private Registrator<ISkill> passiveRegistrator;

		private ICharacter character;

		private ICharacterModel model;
		private PassiveSkill.Factory passiveSkillFactory;
		private BlitzBoltSkill.Factory blitzBoltFactory;

		public Skills(ICharacterModel model, PassiveSkill.Factory passiveSkillFactory, BlitzBoltSkill.Factory blitzBoltFactory)
		{
			this.model = model;
			this.passiveSkillFactory = passiveSkillFactory;
			this.blitzBoltFactory = blitzBoltFactory;

			character = model.Character;

			activeRegistrator = new Registrator<ISkill>();
			passiveRegistrator = new Registrator<ISkill>();

			ActivatePassiveSkills();
			CreateActiveSkills();
		}

		public void PrepareSkill(ISkill skill)
		{
			Assert.IsTrue(IsContainsSkill(skill));
			ActiveSkill = skill as ActiveSkill;
			Assert.IsNotNull(ActiveSkill);
			ActiveSkill.onStatusChanged += OnActiveSkillStatusChanged;

			onActiveSkillChanged?.Invoke();

			ActiveSkill.BeginProcess();
		}

		public void CancelPreparation()
		{
			ActiveSkill?.CancelProcess();
		}

		private void ActivatePassiveSkills()
		{
			character.Sheet.SkillDeck.PassiveSkills.ForEach((data) =>
			{
				PassiveSkill passiveSkill = CreateSkill(data) as PassiveSkill;
				passiveRegistrator.Registrate(passiveSkill);
				passiveSkill.Activate();
			});
		}

		private void CreateActiveSkills()
		{
			character.Sheet.SkillDeck.ActiveSkills.ForEach((data) =>
			{
				ISkill activeSkill = CreateSkill(data);
				activeRegistrator.Registrate(activeSkill);
			});
		}

		private void OnActiveSkillStatusChanged(SkillStatus status)
		{
			if(status == SkillStatus.Canceled || status == SkillStatus.Faulted || status == SkillStatus.Successed)
			{
				if (ActiveSkill != null)
				{
					ActiveSkill.onStatusChanged -= OnActiveSkillStatusChanged;
				}

				ActiveSkill = null;

				onActiveSkillChanged?.Invoke();
			}
		}

		public ISkill CreateSkill(SkillData data)
		{
			if (data is PassiveSkillData passiveSkillData)
			{
				return passiveSkillFactory.Create(passiveSkillData, character);
			}
			else if (data is ActiveSkillData)
			{
				if (data is BlitzBoltData blitzBoltData)
				{
					return blitzBoltFactory.Create(blitzBoltData, character);
				}
			}

			throw new System.NotImplementedException();
		}

		private ISkill GetSkill(SkillData data)
		{
			return activeRegistrator.registers.FirstOrDefault((x) => x.Data == data);
		}

		private bool IsContainsSkill(ISkill skill)
		{
			return activeRegistrator.registers.Contains(skill);
		}
	}

	public sealed class SkillDeck
	{
		public List<SlotSkill> SkillSlots { get; }
		public List<SlotSkill> MemorySkillSlots { get; }

		public List<PassiveSkillData> PassiveSkills { get; }
		public List<ActiveSkillData> ActiveSkills { get; }

		public SkillDeck(SkillsSettings settings)
		{
			SkillSlots = new List<SlotSkill>();
			MemorySkillSlots = new List<SlotSkill>();

			PassiveSkills = settings.skills.Where((skill) => skill is PassiveSkillData).Cast<PassiveSkillData>().ToList();
			ActiveSkills = settings.skills.Where((skill) => skill is ActiveSkillData).Cast<ActiveSkillData>().ToList();
		}

		public SlotSkill[] GetSkillSlotsByLevel(int level)
		{
			return SkillSlots.Where((x) => x.skill.Data.requiredLevel == level).ToArray();
		}

		public List<SkillGroup> GetSkillGroupsByLevel()
		{
			List<SkillGroup> groups = new List<SkillGroup>();

			var levels = SkillSlots.Select((x) => x.skill.Data.requiredLevel).Distinct().OrderBy((y) => y).ToArray();

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