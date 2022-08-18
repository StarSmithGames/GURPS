using Game.Managers.PartyManager;

using Zenject;

namespace Game.Entities.Models
{
    public interface ICompanionModel : ICharacterModel { }

    public class CompanionModel : CharacterModel, ICompanionModel
    {
		public CompanionData data;

		private PartyManager partyManager;

		[Inject]
		private void Construct(PartyManager partyManager)
		{
			this.partyManager = partyManager;
		}

		protected override void OnDestroy()
		{
			partyManager.PlayerParty.RemoveCharacter(Character);

			base.OnDestroy();
		}
		protected override void InitializePersonality()
		{
			Character = new Companion(this, data);
			partyManager.PlayerParty.AddCharacter(Character);
		}
	}
}