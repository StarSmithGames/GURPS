using Game.Systems.SheetSystem;
using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Entities
{
	public abstract class EntityData : ScriptableObject
    {
        public SheetSettings sheet;
    }

	public abstract class CharacterData : EntityData
	{
		[PropertyOrder(-1)]
		[HideLabel]
		public HumanoidInformation information;
	}
}