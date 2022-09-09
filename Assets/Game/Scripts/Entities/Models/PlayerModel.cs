using Game.Managers.FactionManager;

using Sirenix.OdinInspector;

using UnityEngine;
using Zenject;

namespace Game.Entities.Models
{
	public class PlayerModel : HumanoidCharacterModel, IFactionable
	{
		[field: InlineProperty]
		[field: SerializeField] public Faction Faction { get; private set; }

		public CharacterData data;

		public override ICharacter Character
		{
			get
			{
				if(character == null)
				{
					character = new Player(this, data);
				}
				return character;
			}
		}
		private ICharacter character;


		[Inject]
		private void Construct(IPlayer player)
		{
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