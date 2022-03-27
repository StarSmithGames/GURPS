using Game.Managers.CharacterManager;

using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class InventoryContainerHandler : ITickable, IInitializable, IDisposable
	{
		public bool IsDraging { get; private set; }

		public bool IsInventoryOpened { get; private set; }

		private Item item;
		private IInventory from;
		private IInventory to;

		private UIManager uiManager;
		private UIItemCursor itemCursor;
		private CharacterManager characterManager;
		private UIContainerWindow.Factory containerFactory;
		private InputManager inputManager;

		public InventoryContainerHandler(
			UIManager uiManager,
			UIItemCursor itemCursor,
			CharacterManager characterManager,
			UIContainerWindow.Factory containerFactory,
			InputManager inputManager)
		{
			this.uiManager = uiManager;
			this.itemCursor = itemCursor;
			this.characterManager = characterManager;
			this.containerFactory = containerFactory;
			this.inputManager = inputManager;
		}


		public void Initialize()
		{
			IsInventoryOpened = uiManager.CharacterStatus.gameObject.activeSelf;
			CloseCharacterStatus();

			uiManager.CharacterStatus.onClose += CloseCharacterStatus;
		}

		public void Dispose()
		{
			uiManager.CharacterStatus.onClose -= CloseCharacterStatus;
		}

		public void Tick()
		{
			if (inputManager.GetKeyDown(KeyAction.Inventory))
			{
				CharacterStatusEnable();
			}
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

			return containerWindow;
		}


		public void CharacterTakeAllFrom(IInventory inventory)
		{
			from = inventory;
			to = characterManager.CurrentParty.LeaderParty.Inventory;

			for (int i = 0; i < from.Items.Count; i++)
			{
				to.Add(from.Items[i]);
			}
			from.Clear();

			Clear();
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
			if(slot is UISlotInventory inventorySlot)
			{
				if (inventorySlot.IsEmpty) return;
				item = inventorySlot.CurrentItem;
				from = inventorySlot.Owner.CurrentInventory;
				to = characterManager.CurrentParty.LeaderParty.Inventory;
				if (from == to) return;

				to.Add(item);
				from.Remove(item);
			}
		}

		public void OnBeginDrag(UISlot slot, PointerEventData eventData)
		{
			if (slot is UISlotInventory inventorySlot)
			{
				if (inventorySlot.IsEmpty) return;

				characterManager.CurrentParty.LeaderParty.Freeze(true);

				item = inventorySlot.CurrentItem;
				from = inventorySlot.Owner.CurrentInventory;

				itemCursor.SetIcon(inventorySlot.CurrentItem.ItemData.itemSprite);
				itemCursor.transform.parent = uiManager.transform.root;

				IsDraging = true;
			}
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
			Clear();

			characterManager.CurrentParty.LeaderParty.Freeze(false);

			IsDraging = false;
		}
		public void OnDrop(UISlot slot, PointerEventData eventData)
		{
			if (slot is UISlotInventory inventorySlot)
			{
				to = inventorySlot.Owner.CurrentInventory;

				if (from != null && item != null && from != to)
				{
					to.Add(item);
					from.Remove(item);
				}

				characterManager.CurrentParty.LeaderParty.Freeze(false);

				Clear();
			}
		}

		private void Clear()
		{
			itemCursor.Dispose();

			item = null;
			from = null;
			to = null;
		}


		private void CharacterStatusEnable()
		{
			IsInventoryOpened = !IsInventoryOpened;
			uiManager.CharacterStatus.gameObject.SetActive(IsInventoryOpened);
		}
		private void CloseCharacterStatus()
		{
			if (IsInventoryOpened)
			{
				IsInventoryOpened = false;
				uiManager.CharacterStatus.gameObject.SetActive(false);
			}
		}
	}
}