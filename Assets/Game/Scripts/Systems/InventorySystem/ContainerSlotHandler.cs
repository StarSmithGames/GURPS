using Game.Entities;
using Game.HUD;
using Game.Managers.CharacterManager;
using Game.Managers.InputManager;
using Game.Managers.PartyManager;
using Game.Systems.CommandCenter;
using Game.Systems.ContextMenu;
using Game.Systems.SheetSystem;
using Game.Systems.SheetSystem.Skills;
using Game.Systems.TooltipSystem;
using Game.UI.CanvasSystem;

using System;

using UnityEditor.VersionControl;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class ContainerSlotHandler
	{
		public bool IsDraging { get; private set; }

		private WindowCharacterSheet WindowCharacterSheet
		{
			get
			{
				if (windowCharacterSheet == null)
				{
					windowCharacterSheet = subCanvas.WindowsRegistrator.GetAs<WindowCharacterSheet>();
				}

				return windowCharacterSheet;
			}
		}
		private WindowCharacterSheet windowCharacterSheet;

		private UISubCanvas subCanvas;
		private UIDragItem dragItem;
		private UITooltip tooltip;
		private PartyManager partyManager;
		private ContextMenuSystem contextMenuSystem;

		public ContainerSlotHandler(
			UISubCanvas subCanvas,
			UIDragItem itemCursor,
			UITooltip tooltip,
			PartyManager partyManager,
			ContextMenuSystem contextMenuSystem)
		{
			this.subCanvas = subCanvas;
			this.dragItem = itemCursor;
			this.tooltip = tooltip;
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

			tooltip.EnterTarget(slot);
		}
		public void OnPointerExit(UISlot slot, PointerEventData eventData)
		{
			if (IsDraging) return;

			tooltip.ExitTarget(slot);
		}

		public void OnPointerClick(UISlot slot, PointerEventData eventData)
		{
			if (slot.IsEmpty) return;

			switch (slot)
			{
				case UISlotInventory inventorySlot:
				{
					Item item = inventorySlot.Slot.item;

					var from = inventorySlot.Slot.Sheet.Inventory;
					var to = partyManager.PlayerParty.LeaderParty.Sheet.Inventory;

					if (eventData.button == PointerEventData.InputButton.Left)
					{
						if (eventData.clickCount > 1)
						{
							if (from == to)
							{
								if (item.IsEquippable)
								{
									var equipment = (partyManager.PlayerParty.LeaderParty.Sheet as CharacterSheet).Equipment;
									if (equipment.Add(item))
									{
										from.Remove(item);
									}
									else
									{
										//try swap with the first slot
										var slotEquipment = equipment.GetSlotByType(item.ItemData.GetType());
										if(slotEquipment != null)
										{
											Item temp = inventorySlot.Item;
											inventorySlot.Dispose();
											to.Add(slotEquipment.item);
											slotEquipment.SetItem(temp);
										}
									}
								}
								else if (item.IsConsumable)
								{
									CommandConsume.Execute(partyManager.PlayerParty.LeaderParty, item);
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

						tooltip.ExitTarget(slot);
					}
					else if (eventData.button == PointerEventData.InputButton.Right)
					{
						contextMenuSystem.SetTarget(item);

						tooltip.ExitTarget(slot);
					}

					break;
				}
				case UISlotEquipment equipmentSlot:
				{
					if (eventData.clickCount > 1)
					{
						var to = partyManager.PlayerParty.LeaderParty.Sheet.Inventory;

						to.Add(equipmentSlot.Item);
						equipmentSlot.Dispose();

						tooltip.ExitTarget(slot);
					}

					break;
				}
				case UISlotAction actionSlot:
				{
					if (eventData.button == PointerEventData.InputButton.Left)
					{
						if (eventData.clickCount > 0)
						{
							actionSlot.Use();
							tooltip.ExitTarget(slot);
						}
					}
					else if (eventData.button == PointerEventData.InputButton.Right)
					{
						actionSlot.ContextMenu();
						tooltip.ExitTarget(slot);
					}
					else if(eventData.button == PointerEventData.InputButton.Middle)
					{
						actionSlot.Dispose();
						tooltip.ExitTarget(slot);
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

			dragItem.SetIcon(slot.Icon.sprite);
			dragItem.transform.parent = subCanvas.transform;

			IsDraging = true;

			tooltip.ExitTarget(slot);
			partyManager.PlayerParty.LeaderParty.Model.Freeze(true);

			provider = new DragAndDropProvider();
			provider.OnBeginDrag(slot);

			if(slot is UISlotInventory slotInventory)
			{
				if (slotInventory.Item.IsEquippable)
				{
					var equipment = (partyManager.PlayerParty.LeaderParty.Sheet as CharacterSheet).Equipment;
					var slots = equipment.GetSlotsByType(slotInventory.Item.ItemData.GetType());
					WindowCharacterSheet.Equipment.Slots.ForEach((x) =>
					{
						x.EnableProhibition(!slots.Contains(x.Slot));
					});
				}
			}
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

			WindowCharacterSheet.Equipment.Slots.ForEach((x) =>
			{
				x.EnableProhibition(false);
			});

			dragItem.Dispose();
			provider = null;

			partyManager.PlayerParty.LeaderParty.Model.Freeze(false);

			IsDraging = false;
		}
		public void OnDrop(UISlot slot, PointerEventData eventData)
		{
			if (provider == null) return;

			provider.OnDrop(slot);
		}
		#endregion
	}

	public class DragAndDropProvider
	{
		protected UISlot beginSlot = null;

		public void OnBeginDrag(UISlot slot)
		{
			beginSlot = slot;

			Color halfAlpha = beginSlot.Icon.color;
			halfAlpha.a = 0.5f;
			beginSlot.Icon.color = halfAlpha;
		}

		public void OnDrop(UISlot slot)
		{
			if (beginSlot == null) return;
			if (beginSlot == slot) return;

			beginSlot.Drop(slot);
		}

		public void OnEndDrag(UISlot slot)
		{
			//drop in world
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				beginSlot.Dispose();
			}

			Color oneAlpha = beginSlot.Icon.color;
			oneAlpha.a = 1f;
			beginSlot.Icon.color = oneAlpha;
		}
	}


	public static class InventoryDrop
	{
		public static void Process(UISlotInventory begin, UISlot end)
		{
			switch (end)
			{
				case UISlotInventory inventorySlot:
				{
					if (inventorySlot.IsEmpty)
					{
						inventorySlot.SetItem(begin.Item);
						begin.Dispose();
					}
					else
					{
						if (inventorySlot.Item.IsStackable)
						{
							inventorySlot.Item.TryAdd(begin.Item);//
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
					if (equipmentSlot.IsEmpty)
					{
						if (equipmentSlot.SetItem(begin.Item))
						{
							begin.Dispose();
						}
					}
					else
					{
						Item from = begin.Item;
						Item to = equipmentSlot.Item;
						if (equipmentSlot.SetItem(from))
						{
							begin.SetItem(to);
						}
					}

					break;
				}
				case UISlotAction actionSlot:
				{
					if (!begin.Item.IsConsumable) return;

					actionSlot.SetAction(begin.Item);
					break;
				}
			}
		}
	}

	public static class EquipmentDrop
	{
		public static void Process(UISlotEquipment begin, UISlot end)
		{
			switch (end)
			{
				case UISlotInventory inventorySlot:
				{
					if (inventorySlot.IsEmpty)
					{
						inventorySlot.SetItem(begin.Item);
						begin.Dispose();
					}
					else
					{
						inventorySlot.Slot.Sheet.Inventory.Add(begin.Item);
						begin.Dispose();
					}
					break;
				}
				case UISlotEquipment equipmentSlot:
				{
					if (equipmentSlot.IsEmpty)
					{
						if (equipmentSlot.SetItem(begin.Item))
						{
							begin.Dispose();
						}
					}
					else
					{
						//swap
						Item from = begin.Item;
						Item to = equipmentSlot.Item;
						if (equipmentSlot.SetItem(from))
						{
							begin.SetItem(to);
						}
					}
					break;
				}
			}
		}
	}

	public static class ActionBarDrop
	{
		public static void Process(UISlotAction begin, UISlot end)
		{
			switch (end)
			{
				case UISlotAction actionSlot:
				{
					actionSlot.SetAction(begin.Action);
					begin.Dispose();
					break;
				}
			}
		}
	}

	public static class SkillDeckDrop
	{
		public static void Process(UISlotSkill begin, UISlot end)
		{
			switch (end)
			{
				case UISlotAction actionSlot:
				{
					actionSlot.SetAction(begin.Slot.skill);
					break;
				}
			}
		}
	}
}