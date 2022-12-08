using Game.Entities;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public class StunSkill : ActiveSkill
	{
		public override SkillData Data { get; }



		public class Factory : PlaceholderFactory<ActiveSkillData, ICharacter, StunSkill> { }
	}
}