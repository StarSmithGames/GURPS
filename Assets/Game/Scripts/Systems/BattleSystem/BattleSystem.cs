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
	public partial class BattleSystem
	{
		public Turn CurrentTurn { get; private set; }
		public IBattlable CurrentInitiator { get; private set; }
		public Character CachedLeader { get; private set; }

		public bool IsBattleProcess => battleCoroutine != null;
		private Coroutine battleCoroutine = null;
		private bool terminateBattle = false;

		private bool isSkipTurn = false;

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
			AsyncManager asyncManager,
			CharacterManager characterManager,
			CameraController cameraController)
		{
			this.signalBus = signalBus;
			this.settings = settings.battleSettings;
			this.battleManager = battleManager;
			this.asyncManager = asyncManager;
			this.characterManager = characterManager;
			this.cameraController = cameraController;
		}

		private Battle localBattleTest;

		public void StartBattle(List<IBattlable> entities)
		{
			isSkipTurn = false;
			terminateBattle = false;

			CachedLeader = characterManager.CurrentParty.LeaderParty;

			uiManager.Battle.Messages.ShowCommenceBattle();
			uiManager.Battle.SkipTurn.ButtonPointer.onClick.AddListener(StartSkipTurn);
			uiManager.Battle.RunAway.ButtonPointer.onClick.AddListener(StopBattle);

			localBattleTest = new Battle(entities);
			localBattleTest.onNextTurn += OnTurnChanged;
			localBattleTest.onNextRound += uiManager.Battle.Messages.ShowNewRound;
			localBattleTest.onEntitiesChanged += OnEntitiesChanged;

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
			localBattleTest.onEntitiesChanged -= OnEntitiesChanged;

			if (CurrentInitiator != null)
			{
				CurrentInitiator.onDestinationChanged -= OnInitiatorDestinationChanged;
				CurrentInitiator.Sheet.Stats.ActionPoints.onStatChanged -= OnInitiatorActionPointsChanged;
			}

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

			while (!terminateBattle)
			{
				UpdateStates();
				UpdateInitiatorTurn();

				yield return InitiatorTurn();
			}

			yield return WaitInitiatorAction();
			yield return null;//? initiator stuck without frame

			StopBattle();
		}

		private void UpdateStates()
		{
			IEntity initiator = localBattleTest.BattleFSM.CurrentTurn.Initiator;
			bool isEndBattle = localBattleTest.CurrentState == BattleState.EndBattle;

			localBattleTest.Entities.ForEach((x) =>
			{
				if (!isEndBattle)
				{
					//if (characterManager.CurrentParty.Characters.Contains(x))
					//{
					//	if (x == initiator)
					//	{
					//		x.Markers.SetFollowMaterial(MaterialType.Leader);
					//	}
					//	else
					//	{
					//		x.Markers.SetFollowMaterial(MaterialType.Companion);
					//	}
					//}
					//else
					//{
					//	x.Markers.SetFollowMaterial(MaterialType.Enemy);
					//}
				}

				x.Freeze(!isEndBattle);
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
				if (CurrentInitiator.Sheet.Conditions.IsContains<Death>())
				{
					isSkipTurn = true;
				}

				while (isMineTurn && !isSkipTurn)
				{
					if (terminateBattle)
					{
						yield break;
					}
					yield return null;
				}
				isSkipTurn = false;
			}
			else
			{
				Debug.LogError("ENEMY Skip");
				yield return new WaitForSeconds(3f);
			}

			localBattleTest.NextTurn();
		}

		private IEnumerator WaitInitiatorAction()
		{
			yield return new WaitWhile(() => CurrentInitiator.InAction);
		}

		#region Stats Move, ActionsPoints 
		private IEnumerator InitiatorSpendMove()
		{
			IStat stat = CurrentInitiator.Sheet.Stats.Move;

			float from = stat.CurrentValue;
			float to = stat.CurrentValue - CurrentInitiator.Navigation.CurrentNavMeshPathDistance;

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
			IStatBar stat = CurrentInitiator.Sheet.Stats.Move;

			if (stat.CurrentValue < stat.MaxValue)
			{
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

		private void InitiatorRecoveActionsPoints()
		{
			var stat = CurrentInitiator.Sheet.Stats.ActionPoints;
			stat.CurrentValue = stat.MaxValue;

		}
		#endregion

		private void OnEntitiesChanged()
		{
			int countEnemies = 0;
			localBattleTest.Entities.ForEach((x) =>
			{
				if (!characterManager.CurrentParty.Characters.Contains(x))
				{
					countEnemies++;
				}
			});

			terminateBattle = countEnemies == 0;
		}


		private void OnTurnChanged()
		{
			if(CurrentInitiator != null)
			{
				CurrentInitiator.onDestinationChanged -= OnInitiatorDestinationChanged;
				CurrentInitiator.Sheet.Stats.ActionPoints.onStatChanged -= OnInitiatorActionPointsChanged;
			}
			CurrentTurn = localBattleTest.BattleFSM.CurrentTurn;
			CurrentInitiator = CurrentTurn.Initiator;

			InitiatorRecoveActionsPoints();
			InitiatorRecoveMove();

			CurrentInitiator.onDestinationChanged += OnInitiatorDestinationChanged;
			CurrentInitiator.Sheet.Stats.ActionPoints.onStatChanged += OnInitiatorActionPointsChanged;
		}

		private void OnInitiatorDestinationChanged()
		{
			if (CurrentInitiator.IsHasTarget)
			{
				if (!settings.isInfinityMoveStat)
				{
					asyncManager.StartCoroutine(InitiatorSpendMove());
				}
			}
		}

		private void OnInitiatorActionPointsChanged()
		{
			if (settings.isInfinityActionStat)
			{
				var stat = CurrentInitiator.Sheet.Stats.ActionPoints;
				if (stat.CurrentValue != stat.MaxValue)
				{
					stat.CurrentValue = stat.MaxValue;
				}
			}
		}


		[System.Serializable]
		public class Settings
		{
			public bool isInfinityMoveStat = false;
			public bool isInfinityActionStat = false;
		}
	}

	partial class BattleSystem
	{
		public bool IsSkipProcess => skipCoroutine != null;
		private Coroutine skipCoroutine = null;

		private IEnumerator SkipTurn()
		{
			yield return WaitInitiatorAction();
			
			isSkipTurn = true;

			skipCoroutine = null;
		}

		private void StartSkipTurn()
		{
			if (IsSkipProcess) return;

			skipCoroutine = asyncManager.StartCoroutine(SkipTurn());
		}
	}



	public class Battle
	{
		public UnityAction onEntitiesChanged;
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
				x.onDied += OnDied;
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
				x.onDied -= OnDied;
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


		private void OnDied(IEntity entity)
		{
			if (Entities.Contains(entity))
			{
				foreach (var round in BattleFSM.Rounds)
				{
					for (int i = round.Turns.Count - 1; i >= 0; i--)
					{
						var turn = round.Turns[i];

						if (turn.Initiator == entity)
						{
							if (BattleFSM.CurrentTurn == turn)
							{
								NextTurn();
							}

							if (round.Remove(turn))
							{
								onBattleUpdated?.Invoke();
							}
						}
					}
				}
				
				Entities.Remove(entity as IBattlable);
				onEntitiesChanged?.Invoke();
			}
		}
	}

	public class BattleFSM
	{
		public bool isShuffleAllRounds = false;

		public List<Round> Rounds { get; private set; }

		public Round CurrentRound => Rounds.FirstOrDefault();
		public Turn CurrentTurn => CurrentRound.Turns.FirstOrDefault();//max 17 in one round + 1 separator = 18

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

		public bool Remove(Turn turn)
		{
			if (Turns.Contains(turn))
			{
				Turns.Remove(turn);
				return true;
			}

			return false;
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
		public IBattlable Initiator { get; private set; }

		public Turn(IBattlable entity)
		{
			Initiator = entity;
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