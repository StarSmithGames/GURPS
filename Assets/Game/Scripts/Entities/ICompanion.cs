using Game.Entities.Models;
using Game.Systems.SheetSystem;

namespace Game.Entities
{
	public interface ICompanion : ICharacter { }

	public class Companion : Character, ICompanion
	{
		public Companion(ICompanionModel model, CompanionData data)
		{
			Sheet = new CharacterSheet(data);
			Model = model;
		}
	}
}