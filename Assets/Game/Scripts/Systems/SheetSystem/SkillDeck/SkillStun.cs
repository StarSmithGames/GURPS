namespace Game.Systems.SheetSystem.Skills
{
	public class SkillStun : Skill
	{
		public override Skill Copy()
		{
			return Instantiate(this);
		}
	}
}