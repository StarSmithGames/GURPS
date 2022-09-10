using Game.Entities.Models;
using Game.Systems.SheetSystem;

namespace Game.Entities
{
	public interface IEntity<MODEL> where MODEL : IEntityModel
	{
		MODEL Model { get; }
	}

	public interface ICharacter : IEntity<ICharacterModel>, ISheetable { }

	public abstract class Character : ICharacter
	{
		public virtual ISheet Sheet { get; protected set; }
		public virtual ICharacterModel Model { get; protected set; }

		public Character(ICharacterModel model, CharacterData data)
		{
			Sheet = new CharacterSheet(data);
			Model = model;
		}
	}

	/// <summary>
	/// NPC
	/// </summary>
	public class NonPlayableCharacter : Character
	{
		public NonPlayableCharacter(ICharacterModel model, CharacterData data) : base(model, data) { }
	}

	public class PlayableCharacter : Character
	{
		public PlayableCharacter(ICharacterModel model, CharacterData data) : base(model, data) { }
	}
}