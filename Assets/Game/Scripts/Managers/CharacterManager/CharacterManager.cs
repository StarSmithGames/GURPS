using Game.Entities;

namespace Game.Managers.CharacterManager
{
	public class CharacterManager : Registrator<ICharacter>
	{
		public ICharacter Player { get; private set; }

		private Character.Factory characterFactory;

		public CharacterManager(Character.Factory characterFactory)
		{
			this.characterFactory = characterFactory;
		}

		public void CreatePlayer()
		{
			Player = characterFactory.Create(GlobalDatabase.Instance.player);
			Registrate(Player);
		}

		public ICharacter CreateCharacter(CharacterData data)
		{
			ICharacter character = characterFactory.Create(data);
			Registrate(character);
			return character;
		}

		public bool Contains(CharacterData data)
		{
			return registers.Find((x) => x.CharacterData == data) != null;
		}

		public bool Contains(CharacterData data, out ICharacter character)
		{
			character = registers.Find((x) => x.CharacterData == data);
			return character != null;
		}
	}
}