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
	public class ContainerSlotHandler
	{
		public bool IsDraging { get; private set; }

		//private EquipSlot slotEquip;

		private UISubCanvas canvas;
		private UIDragItem dragItem;
		private UITooltip tooltip;
		private UIContainerWindow.Factory containerFactory;
		private PartyManager partyManager;
		private ContextMenuSystem contextMenuSystem;

		public ContainerSlotHandler(
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

		#region Pointer
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
			//var equipment = (initiator.Sheet as CharacterSheet).Equipment;

			switch (slot)
			{
				case UISlotInventory inventorySlot:
				{
					var from = inventorySlot.Slot.CurrentInventory;
					var to = partyManager.PlayerParty.LeaderParty.Sheet.Inventory;

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
									var initiator = partyManager.PlayerParty.LeaderParty;

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

					break;
				}
				case UISlotEquipment equipmentSlot:
				{
					if (eventData.clickCount > 1)
					{
						var to = partyManager.PlayerParty.LeaderParty.Sheet.Inventory;

						to.Add(item);
						//equipment.RemoveFrom(equipmentSlot.CurrentEquip);

						HideTooltip();
					}

					break;
				}
				case UISlotAction actionSlot:
				{
					if (eventData.button == PointerEventData.InputButton.Left)
					{
						if (eventData.clickCount > 1)
						{
							var initiator = partyManager.PlayerParty.LeaderParty;
							var inventory = initiator.Sheet.Inventory;

							if (item.IsConsumable)
							{
								CommandConsume.Execute(initiator, item);
								inventory.Remove(item);
								actionSlot.SetItem(null);
							}
						}
					}
					else if (eventData.button == PointerEventData.InputButton.Right)
					{
						contextMenuSystem.SetTarget(item);

						HideTooltip();
					}
					break;
				}
			}
		}
		#endregion

		#region Drag & Drop
		private DragAndDropProvider provider = null;
		public void OnBeginDrag(UISlot slot, PointerEventData eventData)
		{
			if (slot.IsEmpty) return;
			if (eventData.clickCount > 1) return;

			dragItem.SetIcon(slot.CurrentItem.ItemData.information.portrait);
			dragItem.transform.parent = canvas.transform;

			IsDraging = true;

			HideTooltip();
			partyManager.PlayerParty.LeaderParty.Model.Freeze(true);

			provider = new DragAndDropProvider();
			provider.OnBeginDrag(slot);
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
			provider?.OnEndDrag(slot);

			dragItem.Dispose();

			partyManager.PlayerParty.LeaderParty.Model.Freeze(false);

			IsDraging = false;
		}
		public void OnDrop(UISlot slot, PointerEventData eventData)
		{
			if (provider == null) return;

			provider.OnDrop(slot);
		}
		#endregion

		private void HideTooltip()
		{
			if (tooltip.IsShowing)
			{
				tooltip.Hide();
			}
		}
	}

	public class DragAndDropProvider
	{
		private UISlot beginSlot = null;
		private IInventory to;

		public void OnBeginDrag(UISlot slot)
		{
			beginSlot = slot;
		}

		public void OnDrop(UISlot slot)
		{
			if (beginSlot == null) return;
			if (beginSlot == slot) return;

			to = slot.Slot.CurrentInventory;

			beginSlot.Drop(slot);
		}

		public void OnEndDrag(UISlot slot)
		{
			//drop in world
			if (!EventSystem.current.IsPointerOverGameObject() && to == null)
			{
				beginSlot.Slot.SetItem(null);
			}
		}
	}

	public static class InventoryDrop
	{
		public static void Process(UISlot begin, UISlot end)
		{
			switch (end)
			{
				case UISlotInventory inventorySlot:
				{
					if (inventorySlot.IsEmpty)
					{
						inventorySlot.SetItem(begin.CurrentItem);
						begin.Slot.SetItem(null);
					}
					else
					{
						if (inventorySlot.CurrentItem.IsStackable)
						{
							inventorySlot.CurrentItem.TryAdd(begin.CurrentItem);
						}
						else
						{
							begin.Swap(inventorySlot);
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

				case UISlotAction actionSlot:
				{
					actionSlot.SetItem(begin.CurrentItem);
					break;
				}
			}
		}
	}

	public static class ActionsDrop
	{
		public static void Process(UISlot begin, UISlot end)
		{
			switch (end)
			{
				case UISlotAction actionSlot:
				{
					if (!begin.CurrentItem.IsConsumable) return;

					actionSlot.SetItem(begin.CurrentItem);
					begin.Slot.SetItem(null);

					break;
				}
			}
		}
	}
}