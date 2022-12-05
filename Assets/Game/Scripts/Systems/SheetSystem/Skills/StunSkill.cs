using Game.Entities;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public class StunSkill : Skill
	{
		private StunSkill.Factory factory;

		[Inject]
		private void Construct(StunSkill.Factory factory)
		{
			this.factory = factory;
		}

		private void Start()
		{
			character.Model.ActiveSkillsRegistrator.Registrate(this);
		}

		public override ISkill Create()
		{
			return factory.Create();
		}

		public class Factory : PlaceholderFactory<StunSkill> { }
	}
}