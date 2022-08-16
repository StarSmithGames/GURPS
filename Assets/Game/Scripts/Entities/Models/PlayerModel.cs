using System.Collections;

using Zenject;

namespace Game.Entities.Models
{
	public class PlayerModel : CharacterModel
	{
		private IPlayer player;

		[Inject]
		private void Construct(IPlayer player)
		{
			this.player = player;

			player.Registrate(this);
		}

		protected override void OnDestroy()
		{
			player.UnRegistrate(this);
			base.OnDestroy();
		}

		//protected override void CheckReplicas() { }
	}
}