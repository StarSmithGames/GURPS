using Game.Managers.PartyManager;
using Game.Systems.CommandCenter;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;
using Game.Systems.SheetSystem.Skills;

using Sirenix.OdinInspector;
using Sirenix.Utilities;

using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.HUD
{
	public sealed class UIActionBar : MonoBehaviour
	{
		[field: SerializeField] public Transform Content { get; private set; }
		[SerializeField] private List<UISlotAction> slots = new List<UISlotAction>();

		private IAction usageAction;

		private UISlotAction.Factory actionFactory;
		private PartyManager partyManager;

		[Inject]
		private void Construct(UISlotAction.Factory actionFactory, PartyManager partyManager)
		{
			this.actionFactory = actionFactory;
			this.partyManager = partyManager;
		}

		private void Start()
		{
			var actionBar = partyManager.PlayerParty.LeaderParty.Sheet.ActionBar;

			Assert.IsTrue(actionBar.Slots.Count == slots.Count);

			for (int i = 0; i < slots.Count; i++)
			{
				//Num 0-9
				slots[i].Key.enabled = i < 10;
				slots[i].Key.text = i < 10 ? (i + 1) < 10 ? (i + 1).ToString() : "0" : "";

				slots[i].SetSlot(actionBar.Slots[i]);

				slots[i].onUse += OnUsed;
				slots[i].onChanged += OnSlotChanged;
			}
		}

		private void EnableBlink(IAction action, bool trigger)
		{
			if (trigger)
			{
				slots
					.Where((x) => x.Action == action)
					.ForEach((y) => y.Blink.Do(1f, 0, 0.6f));
			}
			else
			{
				slots
					.Where((x) => x.Action == action)
					.ForEach((y) => y.Blink.Kill(0));
			}
		}

		private void OnUsed(UISlotAction slot)
		{
			var initiator = partyManager.PlayerParty.LeaderParty;

			if (slot.Action is ActiveSkillData skillData)
			{
				bool isRef = false;
				if (initiator.Skills.IsHasPreparedSkill)
				{
					isRef = initiator.Skills.PreparedSkill.Data == slot.Action;

					EnableBlink(initiator.Skills.PreparedSkill.Data, false);
					initiator.Skills.CancelPreparation();
				}

				if (!isRef)
				{
					initiator.Skills.PrepareSkill(skillData);
					EnableBlink(skillData, true);
				}
			}
			else
			{
				if (initiator.Skills.IsHasPreparedSkill)
				{
					EnableBlink(initiator.Skills.PreparedSkill.Data, false);
					initiator.Skills.CancelPreparation();
				}
			}
		}

		private void OnSlotChanged(UISlotAction slot)
		{
			if (slot.IsEmpty)
			{
				slot.Blink.Kill(0);
			}
			else
			{
				var initiator = partyManager.PlayerParty.LeaderParty;

				if (slot.Action is ActiveSkillData skillData)
				{
					if (initiator.Skills.IsHasPreparedSkill)
					{
						if (initiator.Skills.PreparedSkill.Data == skillData)
						{
							slot.Blink.Do(1f, 0, 0.6f);
						}
					}
				}
			}
		}

		[Button(DirtyOnClick = true)]
		private void GetSlots()
		{
			slots = GetComponentsInChildren<UISlotAction>().ToList();
		}
	}
}