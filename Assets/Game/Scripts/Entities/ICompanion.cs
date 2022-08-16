using Game.Entities.Models;
using Game.Managers.CharacterManager;
using Game.Systems.SheetSystem;

namespace Game.Entities
{
	public interface ICompanion : ICharacter
	{

	}

	public class Companion : Character, ICompanion
	{
		public override ISheet Sheet => characterSheet;
		private CharacterSheet characterSheet;

		public override IEntityModel Model => model;
		private ICompanionModel model;

		public Companion(ICompanionModel model, CompanionData data)
		{
			characterSheet = new CharacterSheet(data);
			this.model = model;
		}
	}
}