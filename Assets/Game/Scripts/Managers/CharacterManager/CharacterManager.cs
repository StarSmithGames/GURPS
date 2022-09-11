using Game.Entities;

namespace Game.Managers.CharacterManager
{
	public class CharacterManager
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
		}
	}
}