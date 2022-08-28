using Game.Entities;
using Game.Entities.Models;
using Game.Managers.CharacterManager;
using Game.Managers.PartyManager;
using Game.Systems.SheetSystem;

using System.Collections.Generic;

using UnityEngine;

using Zenject;

public class UIBars : MonoBehaviour
{
	[field: SerializeField] public UIBar HealthBar { get; private set; }
	[field: SerializeField] public UIBar MagicBar { get; private set; }
	[field: SerializeField] public UIBar ArmorBar { get; private set; }
	[field: SerializeField] public UIBar EnergyBar { get; private set; }
	[field: Space]
	[field: SerializeField] public Transform ActionContent { get; private set; }

	private IStatBar actionStat;
	private List<UIAction> actions = new List<UIAction>();

	private SignalBus signalBus;
	private PartyManager partyManager;
	private UIAction.Factory actionFactory;

	[Inject]
	private void Construct(SignalBus signalBus, PartyManager partyManager, UIAction.Factory actionFactory)
	{
		this.signalBus = signalBus;
		this.partyManager = partyManager;
		this.actionFactory = actionFactory;
	}

	private void OnDestroy()
	{
		signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);

		if (actionStat != null)
		{
			actionStat.onStatChanged -= OnStatChanged;
		}
	}

	private void Start()
	{
		signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);

		SetEntity(partyManager.PlayerParty.LeaderParty);
	}

	private void SetEntity(ISheetable sheetable)
	{
		if(actionStat != null)
		{
			actionStat.onStatChanged -= OnStatChanged;
		}

		HealthBar.SetStat(sheetable?.Sheet.Stats.HitPoints);
		EnergyBar.SetStat(sheetable?.Sheet.Stats.Move);

		actionStat = sheetable?.Sheet.Stats.ActionPoints;

		if (actionStat != null)
		{
			actionStat.onStatChanged += OnStatChanged;
		}

		if (actionStat != null)
		{
			if(actionStat.MaxValue != actions.Count)//TODO need resizer
			{
				ActionContent.DestroyChildren();
				actions.Clear();

				for (int i = 0; i < actionStat.MaxValue; i++)
				{
					var action = actionFactory.Create();

					action.transform.parent = ActionContent;

					actions.Add(action);
				}
			}
		}

		OnStatChanged();
	}

	private void OnStatChanged()
	{
		for (int i = 0; i < actions.Count; i++)
		{
			actions[i].Enable(i < actionStat.CurrentValue);
		}
	}

	private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
	{
		SetEntity(signal.leader);
	}
}