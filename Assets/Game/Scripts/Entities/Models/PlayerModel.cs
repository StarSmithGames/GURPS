using Zenject;

namespace Game.Entities.Models
{
	public class PlayerModel : CharacterModel
	{
		[Inject]
		private void Construct(IPlayer player)
		{
			Character = player;
			player.Registrate(this);
		}

		protected override void OnDestroy()
		{
			(Character as IPlayer).UnRegistrate(this);
			base.OnDestroy();
		}

		protected override void CheckReplicas() { }
	}
}