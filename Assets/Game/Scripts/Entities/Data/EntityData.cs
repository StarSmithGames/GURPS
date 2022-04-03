using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
	public class EntityData<T> : ScriptableObject where T : IInformation
    {
        public T information;
        public SheetSettings sheet;
	}
}