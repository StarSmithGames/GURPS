using Game.Managers.CharacterManager;
using Game.Systems.SheetSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class UIBars : MonoBehaviour
{
	[field: SerializeField] public UIBar HealthBar { get; private set; }
	[field: SerializeField] public UIBar MagicBar { get; private set; }
	[field: SerializeField] public UIBar ArmorBar { get; private set; }
	[field: SerializeField] public UIBar EnergyBar { get; private set; }

	private IEntity entity;
	private IStatBar hitPoints;
	private IStatBar move;
	private IStatBar will;

	private SignalBus signalBus;
	private CharacterManager characterManager;

	[Inject]
	private void Construct(SignalBus signalBus, CharacterManager characterManager)
	{
		this.signalBus = signalBus;
		this.characterManager = characterManager;
	}

	private void OnDestroy()
	{
		signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
	}

	private void Start()
	{
		signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);

		SetEntity(characterManager.CurrentParty.LeaderParty);
	}

	private void SetEntity(IEntity entity)
	{
		if (this.entity != null)
		{
			hitPoints.onStatChanged -= OnHitPointsChanged;
			move.onStatChanged -= OnMoveChanged;
			will.onStatChanged -= OnWillChanged;
		}
		this.entity = entity;
		hitPoints = entity.Sheet.Stats.HitPoints;
		move = entity.Sheet.Stats.Move;
		will = entity.Sheet.Stats.Will;

		OnHitPointsChanged();
		OnMoveChanged();
		OnWillChanged();

		if (this.entity != null)
		{
			hitPoints.onStatChanged += OnHitPointsChanged;
			move.onStatChanged += OnMoveChanged;
			will.onStatChanged += OnWillChanged;
		}
	}

	private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
	{
		SetEntity(signal.leader);
	}

	private void OnHitPointsChanged()
	{
		HealthBar.FillAmount = hitPoints.PercentValue;
	}
	private void OnMoveChanged()
	{
		EnergyBar.FillAmount = move.PercentValue;
	}
	private void OnWillChanged()
	{
		MagicBar.FillAmount = will.PercentValue;
	}
}