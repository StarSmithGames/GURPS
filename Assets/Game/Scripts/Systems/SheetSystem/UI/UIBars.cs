using Game.Entities;
using Game.Managers.CharacterManager;
using Game.Systems.SheetSystem;

using System.Collections.Generic;

using UnityEditor.Timeline.Actions;

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
	private CharacterManager characterManager;
	private UIAction.Factory actionFactory;

	[Inject]
	private void Construct(SignalBus signalBus, CharacterManager characterManager, UIAction.Factory actionFactory)
	{
		this.signalBus = signalBus;
		this.characterManager = characterManager;
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

		SetEntity(characterManager.CurrentParty.LeaderParty);
	}

	private void SetEntity(IEntity entity)
	{
		if(actionStat != null)
		{
			actionStat.onStatChanged -= OnStatChanged;
		}

		HealthBar.SetStat(entity?.Sheet.Stats.HitPoints);
		EnergyBar.SetStat(entity?.Sheet.Stats.Move);

		actionStat = entity?.Sheet.Stats.ActionPoints;

		if(actionStat != null)
		{
			actionStat.onStatChanged += OnStatChanged;
		}

		if (actionStat != null)
		{
			if(actionStat.MaxValue != actions.Count)//need resizer
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