using Game.Entities.Models;
using Game.Systems.SheetSystem;

using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Entities
{
	[CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Characters/Character")]
	public class CharacterData : EntityData
	{
		[PropertyOrder(-1)]
		public bool isNPC = true;

		[PropertyOrder(-1)]
		[HideLabel]
		public HumanoidInformation information;

		[AssetSelector]
		public CharacterModel prefab;
	}
}