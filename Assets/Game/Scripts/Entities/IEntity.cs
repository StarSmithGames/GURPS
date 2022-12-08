using Game.Entities.Models;
using Game.Systems.SheetSystem;

using UnityEngine;

using Zenject;

namespace Game.Entities
{
	public interface IEntity<MODEL> where MODEL : IEntityModel
	{
		MODEL Model { get; }
	}

	public interface ICharacter : IEntity<ICharacterModel>, ISheetable
	{
		CharacterData CharacterData { get; }

		new CharacterSheet Sheet { get; }

		void SetModel(ICharacterModel model);

		public Character.Data GetData();
	}

	public class Character : ICharacter
	{
		public CharacterData CharacterData { get; protected set; }
		
		public ISheet Sheet { get; protected set; }
		CharacterSheet ICharacter.Sheet => sheet;
		private CharacterSheet sheet;

		public ICharacterModel Model { get; protected set; }

		public Character(CharacterData data, SheetFactory sheetFactory)
		{
			CharacterData = data;
			//Model = model;
			Sheet = sheetFactory.Create(data);
			Sheet.Restore();

			sheet = Sheet as CharacterSheet;
		}

		public void SetModel(ICharacterModel model)
		{
			Model = model;
		}

		public Data GetData()
		{
			return new Data()
			{
				data = CharacterData,
				sheet = (Sheet as CharacterSheet).GetData(),
			};
		}

		public class Data
		{
			public CharacterData data;
			public CharacterSheet.Data sheet;
		}

		public class Factory : PlaceholderFactory<CharacterData, Character> { }
	}
}