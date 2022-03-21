using Game.Managers.CharacterManager;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Systems.InventorySystem
{
	public class InventoryContainerHandler
	{
		public bool IsDraging { get; private set; }

		private Item item;
		private IInventory from;
		private IInventory to;

		private UIManager uiManager;
		private UIItemCursor itemCursor;
		private CharacterManager characterManager;
		private UIContainerWindow.Factory containerFactory;

		public InventoryContainerHandler(
			UIManager uiManager,
			UIItemCursor itemCursor,
			CharacterManager characterManager,
			UIContainerWindow.Factory containerFactory)
		{
			this.uiManager = uiManager;
			this.itemCursor = itemCursor;
			this.characterManager = characterManager;
			this.containerFactory = containerFactory;
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


		public UIContainerWindow SpawnContainerWindow(IInventory inventory)
		{
			var containerWindow = containerFactory.Create();
			containerWindow.Hide();
			containerWindow.Inventory.SetInventory(inventory);

			containerWindow.transform.parent = uiManager.CurrentVirtualSpace.WindowsRoot;

			(containerWindow.transform as RectTransform).anchoredPosition = Vector3.zero;
			containerWindow.transform.localScale = Vector3.one;
			containerWindow.transform.rotation = Quaternion.Euler(Vector3.zero);

			return containerWindow;
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
			item = slot.CurrentItem;
			from = slot.Owner.CurrentInventory;
			to = characterManager.CurrentCharacter.Inventory;
			if (from == to) return;

			to.Add(item);
			from.Remove(item);
		}

		public void OnBeginDrag(UISlot slot, PointerEventData eventData)
		{
			if (slot.IsEmpty) return;

			IsDraging = true;

			characterManager.CurrentCharacter.Freeze();

			item = slot.CurrentItem;
			from = slot.Owner.CurrentInventory;

			itemCursor.SetItem(slot.CurrentItem);
			itemCursor.transform.parent = uiManager.transform.root;
		}
		public void OnDrag(UISlot slot, PointerEventData eventData)
		{
			if (IsDraging)
			{
				itemCursor.transform.position = eventData.position;
			}
		}
		public void OnEndDrag(UISlot slot, PointerEventData eventData)
		{
			Dispose();

			characterManager.CurrentCharacter.UnFreeze();

			IsDraging = false;
		}
		public void OnDrop(UISlot slot, PointerEventData eventData)
		{
			to = slot.Owner.CurrentInventory;

			if(from != null && item != null && from != to)
			{
				to.Add(item);
				from.Remove(item);
			}

			characterManager.CurrentCharacter.UnFreeze();

			Dispose();
		}

		private void Dispose()
		{
			itemCursor.Dispose();

			item = null;
			from = null;
			to = null;
		}
	}
}