using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
	[CreateAssetMenu(fileName = "Character", menuName = "Game/Character")]
	public class CharacterData : EntityData
	{
        public CharacterInformation information;
	}
}