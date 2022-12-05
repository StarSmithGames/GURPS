using Game.Entities;
using Game.Entities.Models;
using Game.Systems.CameraSystem;
using Game.Systems.CombatDamageSystem;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public interface ISkill { }

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

	//public class ActiveSkillTest : Skill
	//{
	//	public event UnityAction<SkillStatus> onStatusChanged;
	//	public SkillStatus SkillStatus { get; private set; }

	//	public override SkillData Data => data;
	//	private ActiveSkillData data;

	//	public ActiveSkillTest(ActiveSkillData data, ICharacter character, AsyncManager asyncManager) : base(character)
	//	{
	//		this.data = data;

	//		SkillStatus = SkillStatus.None;
	//	}

	//	public void BeginProcess()
	//	{
	//		character.Model.Freeze(true);
	//		character.Model.Markers.EnableSingleTargetLine(true);

	//		SetStatus(SkillStatus.Prepared);
	//	}

	//	public void CancelProcess()
	//	{
	//		character.Model.Markers.EnableSingleTargetLine(false);
	//		character.Model.Freeze(false);

	//		SetStatus(SkillStatus.Canceled);
	//	}

	//	private void SetStatus(SkillStatus status)
	//	{
	//		SkillStatus = status;
	//		onStatusChanged?.Invoke(SkillStatus);
	//	}

	//	public class Factory : PlaceholderFactory<ActiveSkillData, ICharacter, ActiveSkillTest> { }
	//}



	//public class SingleTargetSkill : ActiveSkill
	//{
	//	//private ICharacter character;
	//	//private PassiveSkillData data;

	//	//public SingleTargetSkill(ICharacter character, PassiveSkillData data)
	//	//{
	//	//	this.character = character;
	//	//	this.data = data;
	//	//}

	//	//public class Factory : PlaceholderFactory<ICharacter, ActiveSkillData, ISkill> { }
	//}

	//public class ProjectileSkill : ActiveSkill { }

	//public class AOESkill : ActiveSkill
	//{

	//}

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
				var skill = character.Model.ActiveSkillsRegistrator.registers.Find((x) => x.data == activeSkillData);

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