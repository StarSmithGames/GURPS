using Game.Managers.PartyManager;
using Game.Managers.SceneManager;

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
				if(character == null)
				{
					character = new PlayableCharacter(this, data);
				}
				return character;
			}
		}
		private ICharacter character;

		private PartyManager partyManager;

		[Inject]
		private void Construct(PartyManager partyManager)
		{
			this.partyManager = partyManager;
		}

		protected override void InitializePersonality()
		{
			if (partyManager.PlayerParty.ContainsByData(data))
			{
				DestroyImmediate(gameObject);
			}
		}

		private void OnSceneChanged(SignalSceneChanged signal)
		{
			//partyManager.PlayerParty.RemoveCharacter(Character);
		}
	}
}