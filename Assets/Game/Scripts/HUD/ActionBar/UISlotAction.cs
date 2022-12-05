using DG.Tweening;

using Game.Managers.PartyManager;
using Game.Systems.CommandCenter;
using Game.Systems.ContextMenu;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;
using Game.Systems.SheetSystem.Skills;
using Game.UI;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.HUD
{
	public class UISlotAction : UISlot<SlotAction>
	{
		public UnityAction<UISlotAction> onUse;
		public UnityAction<UISlotAction> onChanged;

		[field: Space]
		[field: SerializeField] public UIAnimationBlinkComponent Blink { get; private set; }
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

		public void Use()
		{
			onUse?.Invoke(this);
		}

		public void ContextMenu()
		{
			if (Action is Item item)
			{
				contextMenuSystem.SetTarget(item);
			}
			else if (Action is SkillData skill)
			{
				contextMenuSystem.SetTarget(skill);
			}
		}

		public bool SetAction(IAction action)
		{
			bool result = Slot.SetAction(action);
			onChanged?.Invoke(this);

			return result;
		}

		public override void Dispose()
		{
			base.Dispose();
			onChanged?.Invoke(this);
		}

		public override void Drop(UISlot slot)
		{
			ActionBarDrop.Drop(this, slot);
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
					case SkillData skill:
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