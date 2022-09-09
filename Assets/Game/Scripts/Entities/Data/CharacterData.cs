using Game.Systems.DialogueSystem;
using Game.Systems.SheetSystem;

using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Entities
{
	[CreateAssetMenu(fileName = "Character", menuName = "Game/Characters/Character")]
	public class CharacterData : EntityData
	{
		[PropertyOrder(-1)]
		[HideLabel]
		public HumanoidInformation information;
	}
}