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
					character = new PlayableCharacter(this, data);
				}
				return character;
			}
		}
		private ICharacter character;


		[Inject]
		private void Construct()
		{

		}

		protected override void CheckReplicas() { }
	}
}