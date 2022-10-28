using Game.Managers.PartyManager;
using Game.Systems.SheetSystem;
using Game.UI;

using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.HUD
{
	public class UIIndicatorsBar : MonoBehaviour
	{
		[field: SerializeField] public UIStatBar HealthBar { get; private set; }
		[field: SerializeField] public UIStatBar MagicBar { get; private set; }
		[field: SerializeField] public UIStatBar ArmorBar { get; private set; }
		[field: SerializeField] public UIStatBar EnergyBar { get; private set; }
		[field: Space]
		[field: SerializeField] public Transform ActionPointsContent { get; private set; }

		private IStatBar actionStat;
		private List<UIActionPoint> actions = new List<UIActionPoint>();

		private SignalBus signalBus;
		private PartyManager partyManager;
		private UIActionPoint.Factory actionPointFactory;

		[Inject]
		private void Construct(SignalBus signalBus, PartyManager partyManager, UIActionPoint.Factory actionPointFactory)
		{
			this.signalBus = signalBus;
			this.partyManager = partyManager;
			this.actionPointFactory = actionPointFactory;
		}

		private void OnDestroy()
		{
			signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);

			if (actionStat != null)
			{
				actionStat.onChanged -= OnStatChanged;
			}
		}

		private void Start()
		{
			signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);

			SetEntity(partyManager.PlayerParty.LeaderParty);
		}

		private void SetEntity(ISheetable sheetable)
		{
			if (actionStat != null)
			{
				actionStat.onChanged -= OnStatChanged;
			}

			HealthBar.SetStat(sheetable?.Sheet.Stats.HitPoints);
			EnergyBar.SetStat(sheetable?.Sheet.Stats.Move);

			actionStat = sheetable?.Sheet.Stats.ActionPoints;

			if (actionStat != null)
			{
				actionStat.onChanged += OnStatChanged;
			}

			if (actionStat != null)
			{
				if (actionStat.MaxValue != actions.Count)//TODO need resizer
				{
					ActionPointsContent.DestroyChildren();
					actions.Clear();

					for (int i = 0; i < actionStat.MaxValue; i++)
					{
						var action = actionPointFactory.Create();

						action.transform.parent = ActionPointsContent;

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
}