using System.Collections;

namespace Game.Entities.Models
{
	public class PlayerModel : CharacterModel
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

		protected override void CheckReplicas() { }
	}
}