using Game.Systems.SheetSystem;
using Game.Systems.SheetSystem.Skills;
using Sirenix.OdinInspector;

using System;
using System.Linq;

using UnityEngine.Events;

namespace Game.Systems.InventorySystem
{
    public abstract class Slot
	{
        public UnityAction onChanged;

        public ISheet Sheet { get; private set; }
        public abstract bool IsEmpty { get; }

        public void SetOwner(ISheet sheet)
        {
            Sheet = sheet;
        }

        public abstract void Dispose();
    }

    [System.Serializable]
    public class SlotInventory : Slot, ICopyable<SlotInventory>
	{
        public override bool IsEmpty => item?.ItemData == null;

		[HideLabel]
        public Item item;

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

        public override void Dispose()
        {
            Sheet.Inventory.RemoveFrom(this);
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
	public class SlotEquipment : Slot, ICopyable<SlotEquipment>
    {
		public override bool IsEmpty => item?.ItemData == null;
        public bool Mark { get; set; }

        [HideLabel]
        public Item item;

        public bool SetItem(Item item)
		{
            if((Sheet as CharacterSheet).Equipment.AddTo(item, this))
			{
                onChanged?.Invoke();
                return true;
			}

            return false;
        }

        public void Swap(SlotEquipment slot)
		{
            Item item = slot.item;
            slot.SetItem(this.item);
            SetItem(item);
		}

		public override void Dispose()
		{
            (Sheet as CharacterSheet).Equipment.RemoveFrom(this);
        }

		public SlotEquipment Copy()
		{
            return new SlotEquipment()
            {
                item = item,
            };
        }

        private string Title => $"Equipment Slot with {item.Title}";
    }

	[System.Serializable]
	public class SlotWeaponEquipment : ICopyable<SlotWeaponEquipment>
	{
        public SlotEquipment main;
        public SlotEquipment spare;

        public SlotWeaponEquipment Copy()
		{
            return new SlotWeaponEquipment()
            {
                main = main.Copy(),
                spare = spare.Copy(),
            };
		}
	}

	[System.Serializable]
    public class SlotAction : Slot, ICopyable<SlotAction>
    {
        public override bool IsEmpty => action == null;

        public IAction action;

        public bool SetAction(IAction action)
		{
            this.action = action;

            onChanged?.Invoke();

            return true;
		}

        public override void Dispose()
        {
            action = null;

            onChanged?.Invoke();
        }

        public SlotAction Copy()
        {
            return new SlotAction()
            {
                action = action,
            };
        }

		private string Title => $"Action Slot with {(action?.GetType().ToString() ?? "NULL")}";
	}

    [System.Serializable]
    public class SlotSkill : Slot, ICopyable<SlotSkill>
    {
        public override bool IsEmpty => skill == null;

        [HideLabel]
        public Skill skill;

        public bool SetSkill(Skill skill)
        {
            this.skill = skill;

            onChanged?.Invoke();

            return true;
        }

        public override void Dispose()
        {
            skill = null;

            onChanged?.Invoke();
        }

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