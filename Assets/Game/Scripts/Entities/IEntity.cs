using Game.Entities.Models;
using Game.Managers.CharacterManager;
using Game.Systems.SheetSystem;

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

		void SetModel(ICharacterModel model);

		public Character.Data GetData();
	}

	public abstract class Character : ICharacter
	{
		public CharacterData CharacterData { get; protected set; }
		public virtual ISheet Sheet { get; protected set; }
		public virtual ICharacterModel Model { get; protected set; }

		public Character(CharacterData data)
		{
			CharacterData = data;
			Sheet = new CharacterSheet(data);
		}

		public Character(Data data)
		{

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
	}

	public class PlayableCharacter : Character
	{
		public PlayableCharacter(CharacterData data) : base(data) { }

		public class Factory : PlaceholderFactory<CharacterData, PlayableCharacter> { }
	}

	public class NonPlayableCharacter : Character
	{
		public NonPlayableCharacter(CharacterData data) : base(data) { }

		public class Factory : PlaceholderFactory<CharacterData, NonPlayableCharacter> { }
	}
}