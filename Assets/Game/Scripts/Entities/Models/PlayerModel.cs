using Game.Managers.CharacterManager;
using Zenject;

namespace Game.Entities.Models
{
	public class PlayerModel : HumanoidCharacterModel
	{
		[Inject]
		private void Construct(CharacterManager characterManager)
		{
			Character = characterManager.Player;
			Character.SetModel(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			Character.SetModel(null);
		}

		protected override void CheckReplicas() { }
	}
}