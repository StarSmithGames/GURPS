using Game.Systems.SheetSystem.Effects;

using System.Collections.Generic;

namespace Game.Systems.InventorySystem
{
    public abstract class ConsumableItemData : ItemData
    {
		public List<EffectData> effects = new List<EffectData>();
	}
}