using Game.Entities.Models;
using Game.Systems.SheetSystem;

namespace Game.Entities
{
	public interface INPC : ICharacter
	{
		NPCData Data { get; }
	}

	public class NPC : Character
	{
		public NPCData Data { get; private set; }

		public override ISheet Sheet
		{
			get
			{
				if (sheet == null)
				{
					sheet = new CharacterSheet(Data);
				}

				return sheet;
			}
		}
		private ISheet sheet = null;

		public NPC(INPCModel model, NPCData data)
		{
			Data = data;
			Model = model;
		}
	}
}