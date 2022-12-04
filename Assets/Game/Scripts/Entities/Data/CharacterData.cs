using Game.Entities.Models;
using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Entities
{
	[CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Characters/Character")]
	public class CharacterData : ModelData
	{
		[PropertyOrder(-1)]
		public bool isNPC = true;

		[AssetSelector]
		public CharacterModel prefab;
	}
}