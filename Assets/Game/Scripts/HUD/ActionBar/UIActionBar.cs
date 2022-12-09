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
using System;

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


			slots[0].SetKeyCode(KeyCode.Alpha1);
			slots[1].SetKeyCode(KeyCode.Alpha2);
			slots[2].SetKeyCode(KeyCode.Alpha3);
			slots[3].SetKeyCode(KeyCode.Alpha4);
			slots[4].SetKeyCode(KeyCode.Alpha5);
			slots[5].SetKeyCode(KeyCode.Alpha6);
			slots[6].SetKeyCode(KeyCode.Alpha7);
			slots[7].SetKeyCode(KeyCode.Alpha8);
			slots[8].SetKeyCode(KeyCode.Alpha9);
			slots[9].SetKeyCode(KeyCode.Alpha0);
			slots[10].SetKeyCode(KeyCode.Minus);
			slots[11].SetKeyCode(KeyCode.Equals);

			for (int i = 0; i < slots.Count; i++)
			{
				slots[i].Key.enabled = slots[i].KeyCode != KeyCode.None;
				slots[i].onClicked += OnClicked;
				slots[i].onChanged += OnSlotChanged;
			}
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

		private void Update()
		{
			for (int i = 0; i < slots.Count; i++)
			{
				if (Input.GetKeyDown(slots[i].KeyCode))
				{
					if (!slots[i].IsEmpty)
					{
						slots[i].Action.Use();
					}
				}
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

		private void OnClicked(UISlotAction slot)
		{
			slot.Action.Use();
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
			if (lastSkill == null) return;

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
			сurrentSkills = currentLeader.LocalSheet.Skills;

			var actionBar = currentLeader.Sheet.ActionBar;

			Assert.IsTrue(actionBar.Slots.Count == slots.Count);

			for (int i = 0; i < slots.Count; i++)
			{
				slots[i].SetSlot(actionBar.Slots[i]);
			}

			сurrentSkills.onActiveSkillChanged += RefreshActiveSkill;
		}

		private void RefreshActiveSkill()
		{
			if (lastSkill != null)
			{
				EnableBlink(lastSkill, false);//bug, null lastSkill
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