using Game.Systems.SheetSystem.Skills;

using Sirenix.OdinInspector;

using UnityEngine.Events;

namespace Game.Systems.InventorySystem
{
    public abstract class Slot
	{
        public UnityAction onChanged;

        public virtual bool IsEmpty => false;
	}

    [System.Serializable]
    public class SlotInventory : Slot, ICopyable<SlotInventory>
	{
        public override bool IsEmpty => item?.ItemData == null;

		[HideLabel]
        public Item item;

        public Inventory CurrentInventory { get; private set; }

        public void SetOwner(Inventory inventory)
        {
            CurrentInventory = inventory;
        }

        public bool SetItem(Item item)
        {
            if (item != null)
            {
                if (item.ItemData == null)
                {
                    item = null;
                }
            }

            this.item = item;

            onChanged?.Invoke();

            return true;
        }

		public SlotInventory Copy()
		{
            return new SlotInventory()
            {
                item = item,
            };
		}

		private string Title => $"Inventory Slot with {item.Title}";
    }

    [System.Serializable]
    public class SlotSkill : Slot, ICopyable<SlotSkill>
    {
        public override bool IsEmpty => skill == null;

        [HideLabel]
        public Skill skill;

        public SlotSkill Copy()
        {
            return new SlotSkill()
            {
                skill = skill,
            };
        }

        private string Title => $"Skill Slot with {skill.Title}";
    }
}