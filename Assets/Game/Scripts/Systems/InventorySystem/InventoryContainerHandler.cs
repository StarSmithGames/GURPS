using Game.Entities;
using Game.Managers.CharacterManager;
using Game.Managers.InputManager;
using Game.Managers.PartyManager;
using Game.Systems.CommandCenter;
using Game.Systems.ContextMenu;
using Game.Systems.SheetSystem;
using Game.Systems.TooltipSystem;

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
		private IEquipment equipment;
		private Equip slotEquip;

		private UIItemCursor itemCursor;
		private UITooltip tooltip;
		private UIContainerWindow.Factory containerFactory;
		private PartyManager partyManager;
		private ContextMenuSystem contextMenuSystem;

		public InventoryContainerHandler(
			UIItemCursor itemCursor,
			UITooltip tooltip,
			UIContainerWindow.Factory containerFactory,
			PartyManager partyManager,
			ContextMenuSystem contextMenuSystem)
		{
			this.itemCursor = itemCursor;
			this.tooltip = tooltip;
			this.containerFactory = containerFactory;
			this.partyManager = partyManager;
			this.contextMenuSystem = contextMenuSystem;
		}

		public void Subscribe(UISlot[] slots)
		{
			for (int i = 0; i < slots.Length; i++)
			{
				UISlot slot = slots[i];
				slot.DragAndDrop.onPointerEnter += OnPointerEnter;
				slot.DragAndDrop.onPointerEXit += OnPointerExit;

				slot.DragAndDrop.onPointerClick += OnPointerClick;

				slot.DragAndDrop.onBeginDrag += OnBeginDrag;
				slot.DragAndDrop.onDrag += OnDrag;
				slot.DragAndDrop.onEndDrag += OnEndDrag;
				slot.DragAndDrop.onDrop += OnDrop;
			}
		}
		public void UnSubscribe(UISlot[] slots)
		{
			for (int i = 0; i < slots.Length; i++)
			{
				UISlot slot = slots[i];
				slot.DragAndDrop.onPointerEnter -= OnPointerEnter;
				slot.DragAndDrop.onPointerEXit -= OnPointerExit;

				slot.DragAndDrop.onPointerClick -= OnPointerClick;

				slot.DragAndDrop.onBeginDrag -= OnBeginDrag;
				slot.DragAndDrop.onDrag -= OnDrag;
				slot.DragAndDrop.onEndDrag -= OnEndDrag;
				slot.DragAndDrop.onDrop -= OnDrop;
			}
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


		public void CharacterTakeAllFrom(IInventory inventory)
		{
			from = inventory;
			//to = characterManager.CurrentParty.LeaderParty.Sheet.Inventory;

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

			item = slot.CurrentItem;

			initiator = partyManager.PlayerParty.LeaderParty;
			equipment = (initiator.Sheet as CharacterSheet).Equipment;

			if (slot is UISlotInventory inventorySlot)
			{
				from = inventorySlot.Owner.CurrentInventory;
				to = partyManager.PlayerParty.LeaderParty.Sheet.Inventory;

				if (eventData.button == PointerEventData.InputButton.Left)
				{
					if (eventData.clickCount > 1)
					{
						if (from == to)
						{
							if (item.IsEquippable)
							{
								equipment.Add(item);
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
				else if(eventData.button == PointerEventData.InputButton.Right)
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
					equipment.RemoveFrom(equipmentSlot.CurrentEquip);

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


			beginSlot = slot;

			//characterManager.CurrentParty.LeaderParty.Freeze(true);

			item = slot.CurrentItem;

			if (slot is UISlotInventory inventorySlot)
			{
				from = inventorySlot.Owner.CurrentInventory;
			}

			//equipment = (characterManager.CurrentParty.LeaderParty.Sheet as CharacterSheet).Equipment;

			itemCursor.SetIcon(item.ItemData.information.portrait);
			//itemCursor.transform.parent = uiManager.transform.root;


			IsDraging = true;
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

			//characterManager.CurrentParty.LeaderParty.Freeze(false);

			IsDraging = false;
		}
		public void OnDrop(UISlot slot, PointerEventData eventData)
		{
			if (item == null) return;
			if (slot == beginSlot) return;

			if (slot is UISlotInventory inventorySlot)
			{
				to = inventorySlot.Owner.CurrentInventory;

				if (from != null && from != to)
				{
					to.Add(item);
					from.Remove(item);
				}
				else if(beginSlot is UISlotEquipment equipmentSlot)
				{
					to.Add(item);
					equipment.RemoveFrom(equipmentSlot.CurrentEquip);
				}
			}
			else if (slot is UISlotEquipment equipmentSlot)
			{
				if (beginSlot is UISlotEquipment neiborSlot)//UISlotEquipment on UISlotEquipment
				{
					equipment.Swap(neiborSlot.CurrentEquip, equipmentSlot.CurrentEquip);
				}
				else//UISlotInventory on UISlotEquipment
				{
					if (from != null)
					{
						if(equipment.AddTo(item, equipmentSlot.CurrentEquip))
						{
							from.Remove(item);
						}
					}
				}
			}

			Clear();
		
			//characterManager.CurrentParty.LeaderParty.Freeze(false);
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
			itemCursor.Dispose();

			item = null;
			from = null;
			to = null;
			equipment = null;

			beginSlot = null;
		}
	}
}