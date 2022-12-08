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
using Game.Managers.GameManager;

namespace Game.HUD
{
	public partial class UIActionBar : MonoBehaviour
	{
		[field: SerializeField] public Transform Content { get; private set; }
		[SerializeField] private List<UISlotAction> slots = new List<UISlotAction>();

		private ICharacter currentLeader;
		private Skills сurrentSkills;
		private ActiveSkill lastSkill;

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
			signalBus?.Subscribe<SignalGameStateChanged>(OnGameStateChanged);


		}

		private void OnDestroy()
		{
			signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
			if(currentLeader != null)
			{
				сurrentSkills.onActiveSkillChanged -= RefreshActiveSkill;
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
				if (сurrentSkills.IsHasActiveSkill)
				{
					isRef = сurrentSkills.ActiveSkill == slot.Action;
					сurrentSkills.CancelPreparation();
				}

				if (!isRef)
				{
					сurrentSkills.PrepareSkill(skillData);
				}
			}
			else
			{
				if (сurrentSkills.IsHasActiveSkill)
				{
					сurrentSkills.CancelPreparation();
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
					if (сurrentSkills.IsHasActiveSkill)
					{
						if (сurrentSkills.ActiveSkill.Data == skillData)
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
				EnableBlink(lastSkill, true);
			}
			else if(status != SkillStatus.Preparing)
			{
				EnableBlink(lastSkill, false);
			}
		}

		private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
		{
			SetLeader(signal.leader);
		}

		private void OnGameStateChanged(SignalGameStateChanged signal)
		{
			if(signal.newGameState == GameState.Gameplay)
			{
				SetLeader(partyManager.PlayerParty.LeaderParty);

				var actionBar = currentLeader.Sheet.ActionBar;

				Assert.IsTrue(actionBar.Slots.Count == slots.Count);

				for (int i = 0; i < slots.Count; i++)
				{
					//Num 0-9
					slots[i].Key.enabled = i < 10;
					slots[i].Key.text = i < 10 ? (i + 1) < 10 ? (i + 1).ToString() : "0" : "";

					slots[i].SetSlot(actionBar.Slots[i]);

					slots[i].onClicked += OnUsed;
					slots[i].onChanged += OnSlotChanged;
				}
			}
		}
	}


	public partial class UIActionBar
	{
		private void SetLeader(ICharacter leader)
		{
			if (currentLeader != null)
			{
				сurrentSkills.onActiveSkillChanged -= RefreshActiveSkill;
			}

			currentLeader = leader;
			сurrentSkills = currentLeader.Model.Skills;

			сurrentSkills.onActiveSkillChanged += RefreshActiveSkill;
		}

		private void RefreshActiveSkill()
		{
			if (lastSkill != null)
			{
				lastSkill.onStatusChanged -= OnActiveSkillStatusChanged;
			}

			lastSkill = сurrentSkills.ActiveSkill;

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