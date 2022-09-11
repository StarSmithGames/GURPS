using Game.Entities;

namespace Game.Managers.CharacterManager
{
	public class CharacterManager : Registrator<ICharacter>
	{
		public ICharacter Player { get; private set; }

		private PlayableCharacter.Factory pcFactory;
		private NonPlayableCharacter.Factory npcFactory;

		public CharacterManager(PlayableCharacter.Factory pcFactory, NonPlayableCharacter.Factory npcFactory)
		{
			this.pcFactory = pcFactory;
			this.npcFactory = npcFactory;
		}

		public void CreatePlayer()
		{
			Player = pcFactory.Create(GlobalDatabase.Instance.player);
			Registrate(Player);
		}

		public ICharacter CreateCharacter(CharacterData data)
		{
			ICharacter character = data is PlayableCharacter ? pcFactory.Create(data) : npcFactory.Create(data) as ICharacter;
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