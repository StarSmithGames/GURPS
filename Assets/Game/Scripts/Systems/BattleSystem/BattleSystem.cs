using Game.Entities;
using Game.Managers.CharacterManager;
using Game.Managers.GameManager;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

using static Game.Systems.BattleSystem.BattleSystem;

namespace Game.Systems.BattleSystem
{
	public class BattleSystem
	{
		public bool IsBattleProcess => battleCoroutine != null;
		private Coroutine battleCoroutine = null;

		private bool isSkipTurn = false;
		private bool isBattleEnd = false;
		private Character cachedCharacter;

		private SignalBus signalBus;
		private BattleManager battleManager;
		private UIManager uiManager;
		private AsyncManager asyncManager;
		private CharacterManager characterManager;
		private CameraController cameraController;

		public BattleSystem(
			SignalBus signalBus,
			BattleManager battleManager,
			UIManager uiManager,
			AsyncManager asyncManager,
			CharacterManager characterManager,
			CameraController cameraController)
		{
			this.signalBus = signalBus;
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

			cachedCharacter = characterManager.CurrentParty.LeaderParty;

			uiManager.Battle.Messages.ShowCommenceBattle();
			uiManager.Battle.SkipTurn.ButtonPointer.onClick.AddListener(SkipTurn);
			uiManager.Battle.RunAway.ButtonPointer.onClick.AddListener(StopBattle);

			localBattleTest = new Battle(entities);
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

			LookAt(cachedCharacter);

			uiManager.Battle.Messages.HideTurnInforamtion();
			uiManager.Battle.SkipTurn.ButtonPointer.onClick.RemoveAllListeners();
			uiManager.Battle.RunAway.ButtonPointer.onClick.RemoveAllListeners();

			UpdateStates();
		}

		private IEnumerator BattleProcess()
		{
			yield return new WaitForSeconds(3f);

			localBattleTest.SetState(BattleState.Battle);

			while (true)
			{
				UpdateStates();
				UpdateInitiatorTurn();

				yield return InitiatorTurn();
			}

			yield return new WaitForSeconds(3f);

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

			LookAt(initiator);

			uiManager.Battle.Messages.ShowTurnInforamtion(isMineTurn ? "YOU TURN" : "ENEMY TURN");

			uiManager.Battle.SkipTurn.Enable(isMineTurn);
			uiManager.Battle.RunAway.Enable(isMineTurn);
		}

		private IEnumerator InitiatorTurn()
		{
			IBattlable initiator = localBattleTest.BattleFSM.CurrentTurn.Initiator;
			bool isMineTurn = characterManager.CurrentParty.Characters.Contains(initiator);

			initiator.Sheet.Stats.RecoveMove();

			if (isMineTurn)
			{
				while (isMineTurn && !isSkipTurn)
				{
					yield return null;
				}

				isSkipTurn = false;
			}
			else
			{
				Debug.LogError("ENEMY SKIP!");
				yield return new WaitForSeconds(3f);
			}

			localBattleTest.NextTurn();
		}


		private void LookAt(IEntity entity)
		{
			if (entity is Character character)
			{
				if (!characterManager.CurrentParty.SetLeader(character))
				{
					cameraController.CameraToHome();
				}
			}
			else
			{
				cameraController.SetFollowTarget(entity.CameraPivot);
			}
		}
	}

	public class Battle
	{
		public UnityAction onBattleUpdated;

		public UnityAction onBattleStateChanged;

		public UnityAction onNextVictory;
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
					onNextVictory?.Invoke();
					onBattleUpdated?.Invoke();
				}
				else
				{
					onNextRound?.Invoke();
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
			return new Round(new List<Turn>(Turns));
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