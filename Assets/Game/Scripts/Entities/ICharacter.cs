using Game.Systems.SheetSystem;

namespace Game.Entities
{
	public interface ICharacter : IEntity
	{

	}

	public class Character : Entity, ICharacter
	{
		public override ISheet Sheet
		{
			get
			{
				if (characterSheet == null)
				{
					//characterSheet = new CharacterSheet(data);
				}

				return characterSheet;
			}
		}
		private CharacterSheet characterSheet;
	}
}