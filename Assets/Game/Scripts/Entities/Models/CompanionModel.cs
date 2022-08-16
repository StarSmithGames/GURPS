using Game.Managers.PartyManager;

using System.Collections;

using Zenject;

namespace Game.Entities.Models
{
    public interface ICompanionModel : ICharacterModel { }

    public class CompanionModel : CharacterModel, ICompanionModel
    {
		public CompanionData data;

		private ICompanion companion;

		private PartyManager partyManager;

		[Inject]
		private void Constuct(PartyManager partyManager)
		{
			this.partyManager = partyManager;
		}

		protected override IEnumerator Start()
		{
			companion = new Companion(this, data);
			partyManager.PlayerParty.AddCharacter(companion);
			return base.Start();
		}

		protected override void OnDestroy()
		{
			partyManager.PlayerParty.RemoveCharacter(companion);
			base.OnDestroy();
		}
	}
}