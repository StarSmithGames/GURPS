using Game.Entities;
using Game.Entities.Models;
using Game.Systems.CombatDamageSystem;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public interface ISkill : IActivation
	{
		SkillData Data { get; }
	}

	public abstract class Skill : ISkill
	{
		public virtual bool IsActive { get; protected set; }
		public virtual SkillData Data { get; protected set; }

		protected ICharacter character;

		public Skill(ICharacter character)
		{
			this.character = character;
		}

		public virtual void Activate()
		{
			IsActive = true;
		}
		public virtual void Deactivate()
		{
			IsActive = false;
		}
	}

	public sealed class PassiveSkill : Skill
	{
		public override SkillData Data => data;
		private PassiveSkillData data;

		private List<Enchantment> enchantments;

		public PassiveSkill(PassiveSkillData data, ICharacter character) : base(character)
		{
			this.data = data;

			enchantments = data.enchantments.Select((x) => x.GetEnchantment(character.Sheet)).ToList();
		}

		public override void Activate()
		{
			base.Activate();

			for (int i = 0; i < enchantments.Count; i++)
			{
				enchantments[i].Activate();
			}
		}

		public override void Deactivate()
		{
			base.Deactivate();

			for (int i = 0; i < enchantments.Count; i++)
			{
				enchantments[i].Deactivate();
			}
		}

		public class Factory : PlaceholderFactory<PassiveSkillData, ICharacter, PassiveSkill> { }
	}

	public class ActiveSkill : Skill
	{
		public override SkillData Data => data;
		private ActiveSkillData data;

		private AsyncManager asyncManager;

		public ActiveSkill(ActiveSkillData data, ICharacter character, AsyncManager asyncManager) : base(character)
		{
			this.data = data;
			this.asyncManager = asyncManager;
		}

		public void BeginProcess()
		{
			character.Model.Markers.EnableSingleTarget(true);
		}

		public void CancelProcess()
		{
			character.Model.Markers.EnableSingleTarget(false);
		}

		public class Factory : PlaceholderFactory<ActiveSkillData, ICharacter, ActiveSkill> { }
	}

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
		private ActiveSkill.Factory activeSkillFactory;

		public CustomSkillFactory(PassiveSkill.Factory passiveSkillFactory, ActiveSkill.Factory activeSkillFactory)
		{
			this.passiveSkillFactory = passiveSkillFactory;
			this.activeSkillFactory = activeSkillFactory;
		}

		public ISkill Create(SkillData data, ICharacter character)
		{
			if (data is PassiveSkillData passiveSkillData)
			{
				return passiveSkillFactory.Create(passiveSkillData, character);
			}
			else if (data is ActiveSkillData activeSkillData)
			{
				return activeSkillFactory.Create(activeSkillData, character);
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