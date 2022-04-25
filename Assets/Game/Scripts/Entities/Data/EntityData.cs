using Game.Systems.DialogueSystem;
using Game.Systems.SheetSystem;

using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Entities
{
	public class EntityData<T> : ScriptableObject where T : IInformation
    {
        public T information;
        public SheetSettings sheet;
    }
}