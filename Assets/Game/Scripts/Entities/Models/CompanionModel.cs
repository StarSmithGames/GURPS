using Game.Managers.CharacterManager;
using Game.Managers.PartyManager;
using Game.Managers.SceneManager;
using Game.Managers.StorageManager;

using Zenject;

namespace Game.Entities.Models
{
    public interface ICompanionModel : ICharacterModel
	{
		
	}

    public class CompanionModel : CharacterModel, ICompanionModel
    {
		public PlayableCharacterData data;

		public override ICharacter Character
		{
			get
			{
				if (character == null)
				{
					character = null;//characterManager.GetCharacter(data, this);
				}
				return character;
			}
		}
		private ICharacter character;

		private CharacterManager characterManager;

		[Inject]
		private void Construct(CharacterManager characterManager)
		{
			this.characterManager = characterManager;
		}
	}
}