using UnityEngine;
using UnityEngine.Events;
using Game.Entities;
using Game.Entities.Models;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	//public class SingleTargetSkill
	//public class ProjectileSkill
	//public class AOESkill
	public abstract class Skill : MonoBehaviour, ISkill
	{
		public event UnityAction<SkillStatus> onStatusChanged;

		public SkillData Data => data;
		[SerializeField] private SkillData data;

		public SkillStatus SkillStatus { get; private set; }

		protected ICharacter character;

		[Inject]
		private void Construct(ICharacterModel model)
		{
			this.character = model.Character;

			SkillStatus = SkillStatus.None;
		}

		protected virtual void Update()
		{
			if (SkillStatus == SkillStatus.Preparing)
			{
				if (Input.GetMouseButtonDown(0))
				{
					Debug.LogError("Fire");
					character.Skills.CancelPreparation();
				}
				else if (Input.GetMouseButtonDown(1))
				{
					character.Skills.CancelPreparation();
				}
			}
		}

		public abstract ISkill Create();

		public virtual void BeginProcess()
		{
			SetStatus(SkillStatus.Preparing);
		}

		public virtual void CancelProcess()
		{
			SetStatus(SkillStatus.Canceled);
		}

		protected void SetStatus(SkillStatus status)
		{
			SkillStatus = status;
			onStatusChanged?.Invoke(SkillStatus);
		}

		public string Title => $"{(data.information.name.IsEmpty() ? name : data.information.name)}";
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
}