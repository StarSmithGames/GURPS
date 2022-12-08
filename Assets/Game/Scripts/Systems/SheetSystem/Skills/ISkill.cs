using Game.Entities;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

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

	public abstract class ActiveSkill : MonoBehaviour, ISkill, IAction
	{
		public event UnityAction<IAction> onUsed;
		public event UnityAction<SkillStatus> onStatusChanged;

		public abstract SkillData Data { get; }

		public SkillStatus SkillStatus { get; private set; }

		[Inject]
		private void Construct()
		{
			//this.character = model.Character;
			SkillStatus = SkillStatus.None;
		}

		protected virtual void Update()
		{
			if (SkillStatus == SkillStatus.Preparing)
			{
				if (Input.GetMouseButtonDown(1))
				{
					//character.Skills.CancelPreparation();
				}
			}
		}

		public bool Use()
		{
			onUsed?.Invoke(this);
			return true;
		}

		public virtual void BeginProcess()
		{
			SetStatus(SkillStatus.Preparing);
		}

		public virtual void CancelProcess()
		{
			SetStatus(SkillStatus.Canceled);
		}

		protected virtual void SetStatus(SkillStatus status)
		{
			SkillStatus = status;
			onStatusChanged?.Invoke(SkillStatus);
		}
	}


	public enum SkillStatus
	{
		None,

		Preparing,
		Running,
		Canceled,
		Faulted,
		Successed,
	}



	//public class SingleTargetSkill
	//public class ProjectileSkill
	//public class AOESkill
	public enum EffectPlacement
	{
		CenteredOnTargets,
		CenteredOnFirstTarget,
		CenteredOnCharacter,
	}
}