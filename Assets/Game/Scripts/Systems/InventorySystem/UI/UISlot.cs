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

		protected bool isHightlight = false;
		private Color one = Color.white;
		private Color oneHalfAlpha = new Color(1, 1, 1, 0.5f);

		public abstract void Dispose();
		public abstract void Drop(UISlot slot);

		public virtual void EnableHightlight(bool trigger)
		{
			isHightlight = trigger;
			Icon.color = isHightlight ? oneHalfAlpha : one;
		}
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

		public virtual void SetSlot(SLOT slot)
		{
			if(Slot != null)
			{
				Slot.onChanged -= OnSlotChanged;
			}

			Slot = slot;

			OnSlotChanged();

			Slot.onChanged += OnSlotChanged;
		}

		public override void Dispose()
		{
			Slot?.Dispose();
		}

		protected abstract void UpdateUI();

		protected virtual void OnSlotChanged()
		{
			UpdateUI();
		}
	}
}