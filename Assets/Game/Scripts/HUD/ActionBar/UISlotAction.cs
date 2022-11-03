using Game.Managers.PartyManager;
using Game.Systems.CommandCenter;
using Game.Systems.ContextMenu;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;
using Game.Systems.SheetSystem.Skills;

using System.Drawing;

using UnityEngine;

using Zenject;

namespace Game.HUD
{
	public class UISlotAction : UISlot<SlotAction>
	{
		[field: Space]
		[field: SerializeField] public TMPro.TextMeshProUGUI Count { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Key { get; private set; }

		public IAction Action => Slot.action;

		private PartyManager partyManager;
		private ContextMenuSystem contextMenuSystem;

		[Inject]
		private void Construct(PartyManager partyManager, ContextMenuSystem contextMenuSystem)
		{
			this.partyManager = partyManager;
			this.contextMenuSystem = contextMenuSystem;
		}

		private void Start()
		{
			containerHandler.Subscribe(this);
		}

		private void OnDestroy()
		{
			containerHandler.Unsubscribe(this);
		}

		public bool Use()
		{
			if(Action is Item item)
			{
				var initiator = partyManager.PlayerParty.LeaderParty;

				if (item.IsConsumable)
				{
					CommandConsume.Execute(initiator, item);

					return true;
				}
			}
			else if (Action is Skill skill)
			{

			}

			return false;
		}

		public void ContextMenu()
		{
			if (Action is Item item)
			{
				contextMenuSystem.SetTarget(item);
			}
			else if (Action is Skill skill)
			{
				contextMenuSystem.SetTarget(skill);
			}
		}

		public bool SetAction(IAction action)
		{
			return Slot.SetAction(action);
		}

		public void Swap(UISlotAction slot)
		{
			IAction action = slot.Slot.action;
			slot.Slot.SetAction(Slot.action);
			Slot.SetAction(action);
		}

		public override void Drop(UISlot slot)
		{
			ActionBarDrop.Process(this, slot);
		}

		protected override void UpdateUI()
		{
			bool isEmpty = IsEmpty;
			
			if (!isEmpty)
			{
				switch (Slot.action)
				{
					case Item item:
					{
						Icon.enabled = true;
						Icon.sprite = item.ItemData.information.portrait;

						Count.enabled = !item.IsArmor && !item.IsWeapon;
						Count.text = item.CurrentStackSize.ToString();
						break;
					}
					case Skill skill:
					{
						Icon.enabled = true;
						Icon.sprite = skill.information.portrait;

						Count.enabled = false;
						Count.text = "";
						break;
					}
				}
			}
			else
			{
				Icon.enabled = false;
				Icon.sprite = null;

				Count.enabled = false;
				Count.text = "";
			}
		}

		public class Factory : PlaceholderFactory<UISlotAction> { }
	}
}