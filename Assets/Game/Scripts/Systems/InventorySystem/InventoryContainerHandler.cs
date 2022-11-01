using Game.Entities;
using Game.Managers.CharacterManager;
using Game.Managers.InputManager;
using Game.Managers.PartyManager;
using Game.Systems.CommandCenter;
using Game.Systems.ContextMenu;
using Game.Systems.SheetSystem;
using Game.Systems.TooltipSystem;
using Game.UI.CanvasSystem;

using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class InventoryContainerHandler
	{
		public bool IsDraging { get; private set; }

		private Item item;
		private IInventory from;
		private IInventory to;

		private ICharacter initiator;
		//private EquipSlot slotEquip;

		private UISubCanvas canvas;
		private UIDragItem dragItem;
		private UITooltip tooltip;
		private UIContainerWindow.Factory containerFactory;
		private PartyManager partyManager;
		private ContextMenuSystem contextMenuSystem;

		public InventoryContainerHandler(
			UISubCanvas canvas,
			UIDragItem itemCursor,
			UITooltip tooltip,
			UIContainerWindow.Factory containerFactory,
			PartyManager partyManager,
			ContextMenuSystem contextMenuSystem)
		{
			this.canvas = canvas;
			this.dragItem = itemCursor;
			this.tooltip = tooltip;
			this.containerFactory = containerFactory;
			this.partyManager = partyManager;
			this.contextMenuSystem = contextMenuSystem;
		}

		public void Subscribe(UISlot slot)
		{
			slot.DragAndDrop.onPointerEnter += OnPointerEnter;
			slot.DragAndDrop.onPointerEXit += OnPointerExit;

			slot.DragAndDrop.onPointerClick += OnPointerClick;

			slot.DragAndDrop.onBeginDrag += OnBeginDrag;
			slot.DragAndDrop.onDrag += OnDrag;
			slot.DragAndDrop.onEndDrag += OnEndDrag;
			slot.DragAndDrop.onDrop += OnDrop;
		}
		public void Unsubscribe(UISlot slot)
		{
			slot.DragAndDrop.onPointerEnter -= OnPointerEnter;
			slot.DragAndDrop.onPointerEXit -= OnPointerExit;

			slot.DragAndDrop.onPointerClick -= OnPointerClick;

			slot.DragAndDrop.onBeginDrag -= OnBeginDrag;
			slot.DragAndDrop.onDrag -= OnDrag;
			slot.DragAndDrop.onEndDrag -= OnEndDrag;
			slot.DragAndDrop.onDrop -= OnDrop;
		}

		public UIContainerWindow SpawnContainerWindow(IInventory inventory)
		{
			var containerWindow = containerFactory.Create();
			containerWindow.Hide();
			containerWindow.Inventory.SetInventory(inventory);
			//containerWindow.transform.parent = uiManager.CurrentVirtualSpace.WindowsRoot;
			(containerWindow.transform as RectTransform).anchoredPosition = Vector3.zero;

			return containerWindow;
		}


		public void OnPointerEnter(UISlot slot, PointerEventData eventData)
		{
			if (IsDraging) return;

			if (!slot.IsEmpty)
			{
				tooltip.SetTarget(slot.CurrentItem);
				tooltip.Show();
			}
		}
		public void OnPointerExit(UISlot slot, PointerEventData eventData)
		{
			if (IsDraging) return;

			HideTooltip();
		}

		public void OnPointerClick(UISlot slot, PointerEventData eventData)
		{
			if (slot.IsEmpty) return;

			Item item = slot.CurrentItem;

			initiator = partyManager.PlayerParty.LeaderParty;
			//var equipment = (initiator.Sheet as CharacterSheet).Equipment;

			if (slot is UISlotInventory inventorySlot)
			{
				from = inventorySlot.Slot.CurrentInventory;
				to = partyManager.PlayerParty.LeaderParty.Sheet.Inventory;

				if (eventData.button == PointerEventData.InputButton.Left)
				{
					if (eventData.clickCount > 1)
					{
						if (from == to)
						{
							if (item.IsEquippable)
							{
								//equipment.Add(item);
								from.Remove(item);
							}
							else if (item.IsConsumable)
							{
								CommandConsume.Execute(initiator, item);
								from.Remove(item);
							}
						}
						else
						{
							to.Add(item);
							from.Remove(item);
						}
					}
					else
					{
						if (from == to) return;

						to.Add(item);
						from.Remove(item);
					}

					HideTooltip();
				}
				else if (eventData.button == PointerEventData.InputButton.Right)
				{
					contextMenuSystem.SetTarget(item);

					HideTooltip();
				}
			}
			else if (slot is UISlotEquipment equipmentSlot)
			{
				if (eventData.clickCount > 1)
				{
					to = partyManager.PlayerParty.LeaderParty.Sheet.Inventory;

					to.Add(item);
					//equipment.RemoveFrom(equipmentSlot.CurrentEquip);

					HideTooltip();
				}
			}

			Clear();
		}


		private UISlot beginSlot = null;
		public void OnBeginDrag(UISlot slot, PointerEventData eventData)
		{
			if (slot.IsEmpty) return;
			if (eventData.clickCount > 1) return;

			HideTooltip();

			beginSlot = slot;
			item = slot.CurrentItem;
			from = GetInventoryFromSlot(beginSlot);

			dragItem.SetIcon(item.ItemData.information.portrait);
			dragItem.transform.parent = canvas.transform;

			partyManager.PlayerParty.LeaderParty.Model.Freeze(true);

			IsDraging = true;
		}
		public void OnDrag(UISlot slot, PointerEventData eventData)
		{
			if (IsDraging)
			{
				dragItem.transform.position = eventData.position;
			}
		}
		public void OnEndDrag(UISlot slot, PointerEventData eventData)
		{
			//drop in world
			if (!EventSystem.current.IsPointerOverGameObject() && to == null)
			{
				beginSlot.Slot.SetItem(null);
			}

			Debug.LogError("Clear");
			Clear();

			partyManager.PlayerParty.LeaderParty.Model.Freeze(false);

			IsDraging = false;
		}
		public void OnDrop(UISlot slot, PointerEventData eventData)
		{
			if (item == null) return;
			if (from == null) return;
			if (beginSlot == slot) return;

			to = GetInventoryFromSlot(slot);

			switch (slot)
			{
				case UISlotInventory inventorySlot:
				{
					if (to != null)
					{
						if (inventorySlot.IsEmpty)
						{
							beginSlot.Slot.SetItem(null);
							inventorySlot.SetItem(item);
						}
						else
						{
							if (inventorySlot.CurrentItem.IsStackable)
							{
								inventorySlot.CurrentItem.TryAdd(item);
							}
							else
							{
								beginSlot.Swap(inventorySlot);
							}
						}
					}
					break;
				}

				case UISlotEquipment equipmentSlot:
				{
					//to.Add(item);
					//equipment.RemoveFrom(equipmentSlot.CurrentEquip);

					//if (beginSlot is UISlotEquipment neiborSlot)//UISlotEquipment on UISlotEquipment
					//{
					//	equipment.Swap(neiborSlot.CurrentEquip, equipmentSlot.CurrentEquip);
					//}
					//else//UISlotInventory on UISlotEquipment
					//{
					//	if (from != null)
					//	{
					//		if (equipment.AddTo(item, equipmentSlot.CurrentEquip))
					//		{
					//			from.Remove(item);
					//		}
					//	}
					//}

					break;
				}
			}
		}

		private void HideTooltip()
		{
			if (tooltip.IsShowing)
			{
				tooltip.Hide();
			}
		}

		private void Clear()
		{
			dragItem.Dispose();

			item = null;
			from = null;
			to = null;

			beginSlot = null;
		}


		private IInventory GetInventoryFromSlot(UISlot slot)
		{
			switch (slot)
			{
				case UISlotInventory inventorySlot:
				{
					return inventorySlot.Slot.CurrentInventory;
				}

				case UISlotEquipment equipmentSlot:
				{
					return equipmentSlot.Slot.CurrentInventory;
				}
			}

			throw new NotImplementedException();
		}
	}
}