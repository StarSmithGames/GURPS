using Game.HUD;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.InventorySystem
{
    public class UISlotSkill : UISlot
    {
        private bool isInitialized = false;

		private void OnDestroy()
		{
			if (isInitialized)
			{
				containerHandler.Unsubscribe(this);
			}
		}

		public override void OnSpawned(IMemoryPool pool)
		{
			if (!isInitialized)
			{
				containerHandler.Subscribe(this);
			}

			isInitialized = true;

			base.OnSpawned(pool);
		}

		public class Factory : PlaceholderFactory<UISlotSkill> { }
    }
}