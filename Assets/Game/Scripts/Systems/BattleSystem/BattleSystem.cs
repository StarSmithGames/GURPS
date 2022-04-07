using Game.Entities;
using Game.Managers.CharacterManager;
using DG.Tweening;
using Game.Systems.CameraSystem;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

using Zenject;
using Game.Systems.SheetSystem;

namespace Game.Systems.BattleSystem
{
	public class BattleSystem
	{
		public Turn CurrentTurn { get; private set; }
		public IEntity CurrentInitiator { get; private set; }
		public Character CachedLeader { get; private set; }

		public bool IsBattleProcess => battleCoroutine != null;
		private Coroutine battleCoroutine = null;

		private bool isSkipTurn = false;
		private bool isBattleEnd = false;

		private SignalBus signalBus;
		private Settings settings;
		private BattleManager battleManager;
		private UIManager uiManager;
		private AsyncManager asyncManager;
		private CharacterManager characterManager;
		private CameraController cameraController;

		public BattleSystem(
			SignalBus signalBus,
			GlobalSettings settings,
			BattleManager battleManager,
			UIManager uiManager,
			AsyncManager asyncManager,
			CharacterManager characterManager,
			CameraController cameraController)
		{
			this.signalBus = signalBus;
			this.settings = settings.battleSettings;
			this.battleManager = battleManager;
			this.uiManager = uiManager;
			this.asyncManager = asyncManager;
			this.characterManager = characterManager;
			this.cameraController = cameraController;
		}

		private Battle localBattleTest;

		public void StartBattle(List<IBattlable> entities)
		{
			isSkipTurn = false;
			isBattleEnd = false;

			CachedLeader = characterManager.CurrentParty.LeaderParty;

			uiManager.Battle.Messages.ShowCommenceBattle();
			uiManager.Battle.SkipTurn.ButtonPointer.onClick.AddListener(SkipTurn);
			uiManager.Battle.RunAway.ButtonPointer.onClick.AddListener(StopBattle);

			localBattleTest = new Battle(entities);
			localBattleTest.onNextTurn += OnTurnChanged;
			localBattleTest.onNextRound += uiManager.Battle.Messages.ShowNewRound;
			battleManager.AddBattle(localBattleTest);
			localBattleTest.Initialization();

			UpdateStates();

			if (!IsBattleProcess)
			{
				battleCoroutine = asyncManager.StartCoroutine(BattleProcess());
			}
		}

		public void StopBattle()
		{
			if (IsBattleProcess)
			{
				asyncManager.StopCoroutine(battleCoroutine);
				battleCoroutine = null;
			}

			localBattleTest.Dispose();
			battleManager.RemoveBattle(localBattleTest);
			localBattleTest.onNextTurn -= OnTurnChanged;
			localBattleTest.onNextRound -= uiManager.Battle.Messages.ShowNewRound;

			uiManager.Battle.Messages.HideTurnInforamtion();
			uiManager.Battle.SkipTurn.ButtonPointer.onClick.RemoveAllListeners();
			uiManager.Battle.RunAway.ButtonPointer.onClick.RemoveAllListeners();

			cameraController.LookAt(CachedLeader);

			UpdateStates();
		}

		private IEnumerator BattleProcess()
		{
			yield return new WaitForSeconds(1.5f);

			localBattleTest.SetState(BattleState.Battle);

			while (true)
			{
				UpdateStates();
				UpdateInitiatorTurn();

				yield return InitiatorTurn();
			}

			yield return new WaitForSeconds(1.5f);

			StopBattle();
		}


		private void SkipTurn()
		{
			isSkipTurn = true;
		}

		private void UpdateStates()
		{
			IEntity initiator = localBattleTest.BattleFSM.CurrentTurn.Initiator;
			bool isEndBattle = localBattleTest.CurrentState == BattleState.EndBattle;

			localBattleTest.Entities.ForEach((x) =>
			{
				x.Freeze(!isEndBattle);

				if (characterManager.CurrentParty.Characters.Contains(x))
				{
					if (x == initiator)
					{
						x.Markers.SetFollowMaterial(MaterialType.Leader);
					}
					else
					{
						x.Markers.SetFollowMaterial(MaterialType.Companion);
					}
				}
				else
				{
					x.Markers.SetFollowMaterial(MaterialType.Enemy);
				}
			});

			if(localBattleTest.CurrentState == BattleState.Battle)
			{
				initiator.Freeze(false);
			}
		}

		private void UpdateInitiatorTurn()
		{
			IEntity initiator = localBattleTest.BattleFSM.CurrentTurn.Initiator;
			bool isMineTurn = characterManager.CurrentParty.Characters.Contains(initiator);

			cameraController.LookAt(initiator);

			uiManager.Battle.Messages.ShowTurnInforamtion(isMineTurn ? "YOU TURN" : "ENEMY TURN");

			uiManager.Battle.SkipTurn.Enable(isMineTurn);
			uiManager.Battle.RunAway.Enable(isMineTurn);
		}

		private IEnumerator InitiatorTurn()
		{
			bool isMineTurn = characterManager.CurrentParty.Characters.Contains(CurrentInitiator);

			if (isMineTurn)
			{
				while (isMineTurn && !isSkipTurn)
				{
					yield return null;
				}

				if(CurrentTurn.Maneuvers.Count == 0 && CurrentInitiator.Sheet.Stats.Move.PercentValue == 1f)
				{
					CurrentTurn.AddManeuver(new Inaction(CurrentInitiator));
				}

				isSkipTurn = false;
			}
			else
			{
				CurrentTurn.AddManeuver(new Wait(CurrentInitiator));
				yield return new WaitForSeconds(3f);
			}

			localBattleTest.NextTurn();
		}

		#region StatMove
		private IEnumerator InitiatorSpendMove()
		{
			IStat stat = CurrentInitiator.Sheet.Stats.Move;

			float from = stat.CurrentValue;
			float to = stat.CurrentValue - CurrentInitiator.Navigation.NavMeshPathDistance;

			NavigationController navigation = CurrentInitiator.Navigation;

			while (navigation.NavMeshAgent.IsReachesDestination())
			{
				stat.CurrentValue = Mathf.Lerp(from, to, navigation.NavMeshInvertedPercentRemainingDistance);

				yield return null;
			}

			stat.CurrentValue = to;
		}

		private void InitiatorRecoveMove(bool isAnimated = true)
		{
			if (CurrentInitiator.Sheet.Stats.Move.CurrentValue < CurrentInitiator.Sheet.Stats.Move.MaxValue)
			{
				IStatBar stat = CurrentInitiator.Sheet.Stats.Move;

				if (isAnimated)
				{
					float from = stat.CurrentValue;
					float to = stat.MaxValue;
				
					DOTween.To(() => from, (x) => stat.CurrentValue = x, to, 0.5f);
				}
				else
				{
					stat.CurrentValue = stat.MaxValue;
				}
			}
		}
		#endregion

		private void OnTurnChanged()
		{
			if(CurrentInitiator != null)
			{
				CurrentInitiator.onDestinationChanged -= OnInitiatorDestinationChanged;
			}
			CurrentTurn = localBattleTest.BattleFSM.CurrentTurn;
			CurrentInitiator = CurrentTurn.Initiator;

			InitiatorRecoveMove();

			CurrentInitiator.onDestinationChanged += OnInitiatorDestinationChanged;
		}

		private void OnInitiatorDestinationChanged()
		{
			if (CurrentInitiator.Controller.IsHasTarget)
			{
				if (!settings.isInfinityMoveStat)
				{
					asyncManager.StartCoroutine(InitiatorSpendMove());
				}
			}
		}


		[System.Serializable]
		public class Settings
		{
			public bool isInfinityMoveStat = false;
		}
	}

	public class Battle
	{
		public UnityAction onBattleUpdated;

		public UnityAction onBattleStateChanged;

		public UnityAction onVictory;
		public UnityAction onNextRound;
		public UnityAction onNextTurn;

		public BattleState CurrentState { get; private set; }
		public BattleFSM BattleFSM { get; private set; }
		public List<IBattlable> Entities { get; private set; }

		public Battle(List<IBattlable> entities)
		{
			BattleFSM = new BattleFSM();
			Entities = entities;
		}

		public void Initialization()
		{
			Entities.ForEach((x) =>
			{
				x.Sheet.Stats.RecoveMove();
				x.JoinBattle(this);
			});

			List<Turn> turns = new List<Turn>();
			Entities.ForEach((x) => turns.Add(new Turn(x)));
			Round round = new Round(turns);
			BattleFSM.Rounds.Add(round);
			BattleFSM.Rounds.Add(round.Copy());

			SetState(BattleState.PreBattle);
			onNextTurn?.Invoke();
		}

		public void Dispose()
		{
			SetState(BattleState.EndBattle);

			Entities.ForEach((x) =>
			{
				x.LeaveBattle();
			});
		}


		public void NextTurn()
		{
			if (!BattleFSM.NextTurn())
			{
				if (!BattleFSM.NextRound())
				{
					onVictory?.Invoke();
					onBattleUpdated?.Invoke();
				}
				else
				{
					onNextRound?.Invoke();
					onNextTurn?.Invoke();
					onBattleUpdated?.Invoke();
				}
			}
			else
			{
				onNextTurn?.Invoke();
				onBattleUpdated?.Invoke();
			}
		}

		public Battle SetState(BattleState state)
		{
			CurrentState = state;

			onBattleStateChanged?.Invoke();
			onBattleUpdated?.Invoke();
			return this;
		}
	}

	public class BattleFSM
	{
		public bool isShuffleAllRounds = false;

		public List<Round> Rounds { get; private set; }

		public Round CurrentRound => Rounds.First();
		public Turn CurrentTurn => CurrentRound.Turns[0];//max 17 in one round + 1 separator = 18

		public BattleFSM()
		{
			Rounds = new List<Round>();
		}

		public bool NextRound()
		{
			if (Rounds.Count > 1)
			{
				Rounds.RemoveAt(0);

				Rounds.Add(isShuffleAllRounds ? Rounds.First().Copy().Shuffle() : Rounds.First().Copy());

				return true;
			}

			return false;
		}

		public bool NextTurn()
		{
			if (CurrentRound.Turns.Count > 1)
			{
				CurrentRound.Turns.RemoveAt(0);

				return true;
			}

			return false;
		}
	}

	public class Round : ICopyable<Round>
	{
		public List<Turn> Turns { get; private set; }

		public Round(List<Turn> turns)
		{
			Turns = turns;
		}

		public Round Shuffle()
		{
			Turns = Turns.OrderBy((x) => Guid.NewGuid()).ToList();//initiative

			return this;
		}

		public Round Copy()
		{
			List<Turn> list = new List<Turn>();
			for (int i = 0; i < Turns.Count; i++)
			{
				list.Add(Turns[i].Copy());
			}

			return new Round(list);
		}
	}
	
	public class Turn : ICopyable<Turn>
	{
		public List<IManeuver> Maneuvers { get; private set; }
		public IBattlable Initiator { get; private set; }

		public Turn(IBattlable entity)
		{
			Maneuvers = new List<IManeuver>();
			Initiator = entity;
		}

		public void AddManeuver(IManeuver maneuver)
		{
			Maneuvers.Add(maneuver);
			maneuver.Execute();
		}

		public bool ContainsManeuver<T>() where T : IManeuver
		{
			return Maneuvers.OfType<T>().Any();
		}

		public Turn Copy()
		{
			return new Turn(Initiator);
		}
	}


	public enum BattleState
	{
		PreBattle,
		Battle,
		EndBattle,
	}
}