using Game.Systems.SheetSystem;
using UnityEngine;

namespace Game.Entities
{
	public abstract class EntityData : ScriptableObject
    {
        public SheetSettings sheet;
    }
}