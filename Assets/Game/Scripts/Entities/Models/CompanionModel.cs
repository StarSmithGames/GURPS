using Game.Managers.PartyManager;

using System.Collections;

using Zenject;

namespace Game.Entities.Models
{
    public interface ICompanionModel : ICharacterModel
	{
		ICompanion Companion { get; }
	}

    public class CompanionModel : CharacterModel, ICompanionModel
    {
		public ICompanion Companion { get; private set; }

		public CompanionData data;

		private PartyManager partyManager;

		[Inject]
		private void Construct(PartyManager partyManager)
		{
			this.partyManager = partyManager;
		}

		protected override void OnDestroy()
		{
			partyManager.PlayerParty.RemoveCharacter(Companion);

			base.OnDestroy();
		}
		protected override void InitializePersonality()
		{
			Companion = new Companion(this, data);
			partyManager.PlayerParty.AddCharacter(Companion);
		}
	}
}