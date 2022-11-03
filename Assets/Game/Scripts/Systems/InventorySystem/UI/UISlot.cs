using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public abstract class UISlot : PoolableObject
	{
		[field: SerializeField] public DragAndDropUISlotComponent DragAndDrop { get; private set; }

		[field: SerializeField] public Image Background { get; protected set; }
		[field: SerializeField] public Image Icon { get; protected set; }

		public virtual bool IsEmpty => true;
		public abstract void Drop(UISlot slot);
	}

	public abstract class UISlot<SLOT> : UISlot
		where SLOT : Slot
	{
		public SLOT Slot { get; private set; }

		public override bool IsEmpty => Slot.IsEmpty;

		protected ContainerSlotHandler containerHandler;

		[Inject]
		private void Construct(ContainerSlotHandler containerHandler)
		{
			this.containerHandler = containerHandler;
		}

		public void SetSlot(SLOT slot)
		{
			if(Slot != null)
			{
				Slot.onChanged -= UpdateUI;
			}

			Slot = slot;

			UpdateUI();

			Slot.onChanged += UpdateUI;
		}

		protected virtual void UpdateUI() { }
	}
}