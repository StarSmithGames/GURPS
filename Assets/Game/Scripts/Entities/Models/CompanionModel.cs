using System.Collections;

namespace Game.Entities.Models
{
    public interface ICompanionModel : ICharacterModel { }

    public class CompanionModel : CharacterModel, ICompanionModel
    {
		protected override IEnumerator Start()
		{
			characterManager.Registrate(this);
			return base.Start();
		}

		protected override void OnDestroy()
		{
			characterManager.UnRegistrate(this);
			base.OnDestroy();
		}
	}
}