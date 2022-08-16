using Game.Systems.DialogueSystem;
using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
	[CreateAssetMenu(fileName = "Character", menuName = "Game/Characters/Character")]
	public class CharacterData : EntityData
	{
        public CharacterInformation information;
		public ActorSettings actorSettings;
	}
}