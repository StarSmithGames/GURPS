using Game.Systems.InventorySystem;
using Zenject;

namespace Game.HUD
{
    public class UISlotSkill : UISlot<SlotSkill>
    {
        private bool isInitialized = false;

		private void OnDestroy()
		{
			if (isInitialized)
			{
				containerHandler.Unsubscribe(this);
			}
		}

		protected override void UpdateUI()
		{
			bool isEmpty = Slot.IsEmpty;
			Icon.enabled = !isEmpty;
			Icon.sprite = !isEmpty ? Slot.skill.information.portrait : null;
		}

		public override void Drop(UISlot slot)
		{
			SkillDeckDrop.Drop(this, slot);
		}

		public override void OnSpawned(IMemoryPool pool)
		{
			if (!isInitialized)
			{
				containerHandler.Subscribe(this);
				isInitialized = true;
			}

			base.OnSpawned(pool);
		}

		public class Factory : PlaceholderFactory<UISlotSkill> { }
    }
}