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
		Effects Effects { get; }

		void SetModel(ICharacterModel model);

		public Character.Data GetData();
	}

	public class Character : ICharacter
	{
		public CharacterData CharacterData { get; protected set; }
		public virtual ISheet Sheet { get; protected set; }
		public virtual ICharacterModel Model { get; protected set; }

		public virtual Effects Effects { get; private set; }

		public Character(CharacterData data, EffectFactory effectFactory)
		{
			CharacterData = data;
			Sheet = new CharacterSheet(data);
			Effects = new Effects(this, effectFactory);
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