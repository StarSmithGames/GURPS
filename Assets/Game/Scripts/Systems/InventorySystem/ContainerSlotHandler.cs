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
					Item item = inventorySlot.Item;

					var from = inventorySlot.Slot.Sheet.Inventory;
					var to = partyManager.PlayerParty.LeaderParty.Sheet.Inventory;

					if (eventData.button == PointerEventData.InputButton.Left)
					{
						if (eventData.clickCount > 1)
						{
							InventoryPointer.DoubleClick(inventorySlot, partyManager.PlayerParty.LeaderParty);
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
						EquipmentPointer.DoubleClick(equipmentSlot, partyManager.PlayerParty.LeaderParty);

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
			beginSlot.EnableHightlight(true);
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

			beginSlot.EnableHightlight(false);
		}
	}


	public static class InventoryPointer
	{
		public static void DoubleClick(UISlotInventory begin, ICharacter initiator)
		{
			Item item = begin.Item;

			var from = begin.Slot.Sheet.Inventory;
			var to = initiator.Sheet.Inventory;

			if (from == to)
			{
				if (item.IsEquippable)
				{
					var equipment = (initiator.Sheet as CharacterSheet).Equipment;

					if (item.IsArmor)
					{
						if (equipment.Add(item))
						{
							from.Remove(item);
						}
						else
						{
							//try swap with the first slot
							var slotEquipment = equipment.GetSlotByType(item.ItemData.GetType());
							if (slotEquipment != null)
							{
								begin.Dispose();
								to.Add(slotEquipment.item);
								slotEquipment.SetItem(item);
							}
						}
					}
					else if(item.IsWeapon)
					{
						var weaponSlot = equipment.CurrentWeapon;

						from.Remove(item);//no care where

						if (item.IsTwoHandedWeapon)
						{
							weaponSlot.PutOnTwoHandedWeaponFrom(item, from);
						}
						else
						{
							if (weaponSlot.main.IsEmpty)
							{
								weaponSlot.main.SetItem(item);
							}
							else if (weaponSlot.spare.IsEmpty)
							{
								weaponSlot.spare.SetItem(item);
							}
							else
							{
								weaponSlot.TakeOffTwoHandedWeaponTo(from);

								if (!weaponSlot.main.IsEmpty)
								{
									from.Add(weaponSlot.main.item);
								}
								weaponSlot.main.SetItem(item);
							}
						}
					}
				}
				else if (item.IsConsumable)
				{
					CommandConsume.Execute(initiator, item);
				}
			}
			else
			{
				to.Add(item);
				from.Remove(item);
			}
		}

		public static void Drop(UISlotInventory begin, UISlot end)
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
					var from = begin.Slot.Sheet.Inventory;
					Item item = begin.Item;

					if (item.IsArmor)
					{
						if (equipmentSlot.IsEmpty)
						{
							if (equipmentSlot.SetItem(item))
							{
								begin.Dispose();
							}
						}
						else
						{

							Item to = equipmentSlot.Item;
							if (equipmentSlot.SetItem(item))
							{
								begin.SetItem(to);
							}
						}
					}
					else if (item.IsWeapon)
					{
						var weaponSlot = (equipmentSlot.Slot.Sheet as CharacterSheet).Equipment.GetWeaponSlot(equipmentSlot.Slot);
						begin.Slot.Sheet.Inventory.Remove(item);//no care where

						if (item.IsTwoHandedWeapon)
						{
							weaponSlot.TakeOffWeaponTo(from);
							weaponSlot.SetItem(item);
						}
						else
						{
							weaponSlot.TakeOffTwoHandedWeaponTo(from);

							if (!equipmentSlot.IsEmpty)
							{
								from.Add(equipmentSlot.Item);
							}
							equipmentSlot.SetItem(item);
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

	public static class EquipmentPointer
	{
		public static void DoubleClick(UISlotEquipment begin, ICharacter initiator)
		{
			begin.Slot.TakeOffTo(initiator.Sheet.Inventory);
		}

		public static void Drop(UISlotEquipment begin, UISlot end)
		{
			switch (end)
			{
				case UISlotInventory inventorySlot:
				{
					Item from = begin.Item;
					if (from.IsArmor)
					{
						if (inventorySlot.IsEmpty)
						{
							inventorySlot.SetItem(from);
							begin.Dispose();
						}
						else
						{
							inventorySlot.Slot.Sheet.Inventory.Add(from);
							begin.Dispose();
						}
					}
					else if (from.IsWeapon)
					{
						begin.Slot.TakeOffTo(inventorySlot.Slot);
					}
					break;
				}
				case UISlotEquipment equipmentSlot:
				{
					Item from = begin.Item;
					Item to = equipmentSlot.Item;

					if (from.IsArmor)
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
							if (equipmentSlot.SetItem(from))
							{
								begin.SetItem(to);
							}
						}
					}
					else if(from.IsWeapon)//swap weapons
					{
						var weaponFrom = (begin.Slot.Sheet as CharacterSheet).Equipment.GetWeaponSlot(begin.Slot);
						var weaponTo = (equipmentSlot.Slot.Sheet as CharacterSheet).Equipment.GetWeaponSlot(equipmentSlot.Slot);

						if (weaponFrom == weaponTo)
						{
							weaponFrom.Swap();
						}
						else
						{
							if (from.IsTwoHandedWeapon)
							{
								weaponFrom.Swap(weaponTo);
							}
							else
							{
								if (equipmentSlot.IsEmpty)
								{
									begin.Slot.Swap(equipmentSlot.Slot);
								}
								else
								{
									if (to.IsTwoHandedWeapon)
									{
										weaponFrom.Swap(weaponTo);
									}
									else
									{
										begin.Slot.Swap(equipmentSlot.Slot);
									}
								}
							}
						}

						if (from.IsTwoHandedWeapon)
						{
							if (equipmentSlot.IsEmpty)
							{

							}
							else
							{

							}
						}
						else
						{
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