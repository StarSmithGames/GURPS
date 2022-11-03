namespace Game.Systems.SheetSystem.Skills
{
	public class SkillSpringAttack : Skill
	{
		public override Skill Copy()
		{
			return Instantiate(this);
		}
	}
}