using DG.Tweening;

using FlowCanvas.Nodes;

using Game.Managers.InputManager;
using Game.Managers.PartyManager;
using Game.Systems.CommandCenter;
using Game.Systems.ContextMenu;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;
using Game.Systems.SheetSystem.Skills;
using Game.UI;

using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.HUD
{
	public class UISlotAction : UISlot<SlotAction>
	{
		public UnityAction<UISlotAction> onClicked;
		public UnityAction<UISlotAction> onChanged;

		[field: Space]
		[field: SerializeField] public Image FillAmountCooldown { get; private set; }
		[field: SerializeField] public UIAnimationBlinkComponent Blink { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Count { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Key { get; private set; }

		public KeyCode KeyCode { get; private set; }

		public IAction Action => Slot.action;

		private InputManager inputManager;
		private PartyManager partyManager;
		private ContextMenuSystem contextMenuSystem;

		[Inject]
		private void Construct(InputManager inputManager, PartyManager partyManager, ContextMenuSystem contextMenuSystem)
		{
			this.inputManager = inputManager;
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

		public void OneClick()
		{
			onClicked?.Invoke(this);
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

		public void SetKeyCode(KeyCode code)
		{
			KeyCode = code;
			Key.text = inputManager.GetKeyName(KeyCode);
		}

		public override void Dispose()
		{
			if (!IsEmpty)
			{
				if (Action is ActiveSkill activeSkill)
				{
					activeSkill.Cooldown.onChanged -= OnCooldownChanged;
				}
			}
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
						FillAmountCooldown.enabled = false;

						Icon.enabled = true;
						Icon.sprite = item.ItemData.information.portrait;

						Count.enabled = !item.IsArmor && !item.IsWeapon;
						Count.text = item.CurrentStackSize.ToString();
						break;
					}
					case ISkill skill:
					{
						FillAmountCooldown.fillAmount = 0f;
						FillAmountCooldown.enabled = true;

						Icon.enabled = true;
						Icon.sprite = skill.Data.information.portrait;

						Count.enabled = false;
						Count.text = "";
						break;
					}
					default:
					{
						throw new NotImplementedException();
					}
				}
			}
			else
			{
				FillAmountCooldown.enabled = false;

				Icon.enabled = false;
				Icon.sprite = null;

				Count.enabled = false;
				Count.text = "";
			}
		}

		protected override void OnSlotChanged()
		{
			base.OnSlotChanged();

			if (!IsEmpty)
			{
				if (Action is ActiveSkill activeSkill)
				{
					activeSkill.Cooldown.onChanged -= OnCooldownChanged;
				}
			}

			if (Action is ActiveSkill skill)
			{
				skill.Cooldown.onChanged += OnCooldownChanged;
			}
		}

		private void OnCooldownChanged(float cooldown, float cooldownNormalized)
		{
			FillAmountCooldown.fillAmount = cooldownNormalized;
		}

		public class Factory : PlaceholderFactory<UISlotAction> { }
	}
}