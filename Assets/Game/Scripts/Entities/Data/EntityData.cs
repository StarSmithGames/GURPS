using Game.Systems.SheetSystem;
using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Entities
{
    public abstract class EntityData<I> : ScriptableObject where I : Information
	{
		[HideLabel]
		public I information;
		public SheetSettings sheet;
    }
}