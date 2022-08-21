using Game.Managers.FactionManager;

using Sirenix.OdinInspector;

using UnityEngine;
using Zenject;

namespace Game.Entities.Models
{
	public class PlayerModel : CharacterModel, IFactionable
	{
		[field: InlineProperty]
		[field: SerializeField] public Faction Faction { get; private set; }

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