using Game.Entities;
using Game.Managers.PartyManager;
using Game.Systems.SheetSystem;
using Game.Systems.SheetSystem.Skills;
using System.Collections.Generic;
using System.Linq;

using Sirenix.OdinInspector;
using Sirenix.Utilities;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.HUD
{
	public partial class UIActionBar : MonoBehaviour
	{
		[field: SerializeField] public Transform Content { get; private set; }
		[SerializeField] private List<UISlotAction> slots = new List<UISlotAction>();

		private ICharacter currentLeader;
		private Skill lastSkill;

		private SignalBus signalBus;
		private UISlotAction.Factory actionFactory;
		private PartyManager partyManager;

		[Inject]
		private void Construct(SignalBus signalBus, UISlotAction.Factory actionFactory, PartyManager partyManager)
		{
			this.signalBus = signalBus;
			this.actionFactory = actionFactory;
			this.partyManager = partyManager;
		}

		private void Start()
		{
			signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
			SetLeader(partyManager.PlayerParty.LeaderParty);

			var actionBar = currentLeader.Sheet.ActionBar;

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

		private void OnDestroy()
		{
			signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
			if(currentLeader != null)
			{
				currentLeader.Skills.onActiveSkillChanged -= onActiveSkillChanged;
			}
			if (lastSkill != null)
			{
				lastSkill.onStatusChanged -= OnActiveSkillStatusChanged;
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
			if (slot.Action is SkillData skillData)
			{
				bool isRef = false;
				if (currentLeader.Skills.IsHasActiveSkill)
				{
					isRef = currentLeader.Skills.ActiveSkill.Data == slot.Action;
					currentLeader.Skills.CancelPreparation();
				}

				if (!isRef)
				{
					currentLeader.Skills.PrepareSkill(skillData);
				}
			}
			else
			{
				if (currentLeader.Skills.IsHasActiveSkill)
				{
					currentLeader.Skills.CancelPreparation();
				}
			}
		}

		//rm
		private void OnSlotChanged(UISlotAction slot)
		{
			if (slot.IsEmpty)
			{
				slot.Blink.Kill(0);
			}
			else
			{
				if (slot.Action is ActiveSkillData skillData)
				{
					if (currentLeader.Skills.IsHasActiveSkill)
					{
						if (currentLeader.Skills.ActiveSkill.Data == skillData)
						{
							slot.Blink.Do(1f, 0, 0.6f);
						}
					}
				}
			}
		}

		private void OnActiveSkillStatusChanged(SkillStatus status)
		{
			if(status == SkillStatus.Preparing)
			{
				EnableBlink(lastSkill.Data, true);
			}
			else if(status != SkillStatus.Preparing)
			{
				EnableBlink(lastSkill.Data, false);
			}
		}

		private void onActiveSkillChanged()
		{
			RefreshActiveSkill();
		}
		private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
		{
			SetLeader(signal.leader);
		}
	}


	public partial class UIActionBar
	{
		private void SetLeader(ICharacter leader)
		{
			if (currentLeader != null)
			{
				currentLeader.Skills.onActiveSkillChanged -= onActiveSkillChanged;
			}

			currentLeader = leader;

			currentLeader.Skills.onActiveSkillChanged += onActiveSkillChanged;
		}

		private void RefreshActiveSkill()
		{
			if (lastSkill != null)
			{
				lastSkill.onStatusChanged -= OnActiveSkillStatusChanged;
			}

			lastSkill = currentLeader.Skills.ActiveSkill;

			if (lastSkill != null)
			{
				lastSkill.onStatusChanged += OnActiveSkillStatusChanged;

				OnActiveSkillStatusChanged(lastSkill.SkillStatus);
			}
		}

		[Button(DirtyOnClick = true)]
		private void GetSlots()
		{
			slots = GetComponentsInChildren<UISlotAction>().ToList();
		}
	}
}