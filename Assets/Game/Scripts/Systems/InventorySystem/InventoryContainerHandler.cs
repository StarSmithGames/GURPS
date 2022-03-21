using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Systems.InventorySystem
{
	public class InventoryContainerHandler
	{
		public bool IsDraging { get; private set; }

		private UIManager uiManager;
		private UIItemCursor itemCursor;

		public InventoryContainerHandler(UIManager uiManager, UIItemCursor itemCursor)
		{
			this.uiManager = uiManager;
			this.itemCursor = itemCursor;
		}

		public void Subscribe(UIInventory uiInventory)
		{
			uiInventory.slots.ForEach((x) =>
			{
				x.DragAndDrop.onPointerEnter += OnPointerEnter;
				x.DragAndDrop.onPointerEXit += OnPointerExit;

				x.DragAndDrop.onPointerClick += OnPointerClick;

				x.DragAndDrop.onBeginDrag += OnBeginDrag;
				x.DragAndDrop.onDrag += OnDrag;
				x.DragAndDrop.onEndDrag += OnEndDrag;
				x.DragAndDrop.onDrop += OnDrop;
			});
		}
		public void UnSubscribe(UIInventory uiInventory)
		{
			uiInventory.slots.ForEach((x) =>
			{
				x.DragAndDrop.onPointerEnter -= OnPointerEnter;
				x.DragAndDrop.onPointerEXit -= OnPointerExit;

				x.DragAndDrop.onPointerClick -= OnPointerClick;

				x.DragAndDrop.onBeginDrag -= OnBeginDrag;
				x.DragAndDrop.onDrag -= OnDrag;
				x.DragAndDrop.onEndDrag -= OnEndDrag;
				x.DragAndDrop.onDrop -= OnDrop;
			});
		}


		public void OnPointerEnter(UISlot slot, PointerEventData eventData)
		{
			if (IsDraging) return;
		}
		public void OnPointerExit(UISlot slot, PointerEventData eventData)
		{
			if (IsDraging) return;
		}

		public void OnPointerClick(UISlot slot, PointerEventData eventData)
		{
			if (slot.IsEmpty) return;
		}

		public void OnBeginDrag(UISlot slot, PointerEventData eventData)
		{
			if (slot.IsEmpty) return;

			IsDraging = true;

			itemCursor.SetSlot(slot);
			itemCursor.SetItem(slot.CurrentItem);
			itemCursor.transform.parent = uiManager.transform.root;
		}
		public void OnDrag(UISlot slot, PointerEventData eventData)
		{
			if (IsDraging)
			{
				if (slot.IsEmpty) return;

				itemCursor.transform.position = eventData.position;
			}
		}
		public void OnEndDrag(UISlot slot, PointerEventData eventData)
		{
			itemCursor.Dispose();

			IsDraging = false;
		}
		public void OnDrop(UISlot slot, PointerEventData eventData)
		{
			if (itemCursor.Slot != slot)
			{
				itemCursor.Slot.SetItem(null);
				slot.SetItem(itemCursor.Item);
			}

			itemCursor.Dispose();
		}
	}
}