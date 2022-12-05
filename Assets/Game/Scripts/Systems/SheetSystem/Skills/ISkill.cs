using Game.Entities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public interface ISkill
	{
		SkillData Data { get; }
	}

	public sealed class PassiveSkill : ISkill, IActivation
	{
		public bool IsActive { get; private set; }

		private List<Enchantment> enchantments;

		public SkillData Data => data;
		private PassiveSkillData data;
		private ICharacter character;

		public PassiveSkill(PassiveSkillData data, ICharacter character)
		{
			this.data = data;
			this.character = character;

			enchantments = data.enchantments.Select((x) => x.GetEnchantment(character.Sheet)).ToList();
		}

		public void Activate()
		{
			IsActive = true;

			for (int i = 0; i < enchantments.Count; i++)
			{
				enchantments[i].Activate();
			}
		}

		public void Deactivate()
		{
			IsActive = false;

			for (int i = 0; i < enchantments.Count; i++)
			{
				enchantments[i].Deactivate();
			}
		}

		public class Factory : PlaceholderFactory<PassiveSkillData, ICharacter, PassiveSkill> { }
	}

	public class SkillFactory : PlaceholderFactory<SkillData, ICharacter, ISkill> { }

	public class CustomSkillFactory : IFactory<SkillData, ICharacter, ISkill>
	{
		private PassiveSkill.Factory passiveSkillFactory;

		public CustomSkillFactory(PassiveSkill.Factory passiveSkillFactory)
		{
			this.passiveSkillFactory = passiveSkillFactory;
		}

		public ISkill Create(SkillData data, ICharacter character)
		{
			if (data is PassiveSkillData passiveSkillData)
			{
				return passiveSkillFactory.Create(passiveSkillData, character);
			}
			else if (data is ActiveSkillData activeSkillData)
			{
				var skill = character.Model.ActiveSkillsRegistrator.registers.Find((x) => x.Data == activeSkillData);

				Assert.IsNotNull(skill);

				return skill.Create();
			}

			throw new System.NotImplementedException();
		}
	}

	public enum EffectPlacement
	{
		CenteredOnTargets,
		CenteredOnFirstTarget,
		CenteredOnCharacter,
	}
}