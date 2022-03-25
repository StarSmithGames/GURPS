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

using static Game.Systems.BattleSystem.BattleSystem;

namespace Game.Systems.BattleSystem
{
	public class BattleSystem
	{
		public Battle CurrentBattle { get; private set; }

		private List<Battle> currentBattles = new List<Battle>();

		public bool IsBattleProcess => battleCoroutine != null;
		private Coroutine battleCoroutine = null;

		private bool isSkipTurn = false;
		private bool isBattleEnd = false;
		private Character cachedCharacter;


		private GameManager gameManager;
		private UIManager uiManager;
		private AsyncManager asyncManager;
		private CharacterManager characterManager;
		private CameraController cameraController;

		public BattleSystem(GameManager gameManager, UIManager uiManager, AsyncManager asyncManager, CharacterManager characterManager, CameraController cameraController)
		{
			this.gameManager = gameManager;
			this.uiManager = uiManager;
			this.asyncManager = asyncManager;
			this.characterManager = characterManager;
			this.cameraController = cameraController;
		}

		public void StartBattle(List<IEntity> entities)
		{
			gameManager.ChangeState(GameState.PreBattle);

			isSkipTurn = false;
			isBattleEnd = false;

			Battle battle = new Battle()
			{
				entities = entities,
			};

			battle.CreateStartRounds();

			CurrentBattle = battle;
			currentBattles.Add(battle);

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

			BattlePhaseCompletion();

			gameManager.ChangeState(GameState.Gameplay);
		}


		private IEnumerator BattleProcess()
		{
			BattlePhaseInitialization();

			yield return new WaitForSeconds(3f);

			gameManager.ChangeState(GameState.Battle);

			while (true)
			{
				UpdateStates();
				UpdateInitiatorTurn();

				yield return InitiatorTurn();
			}

			gameManager.ChangeState(GameState.EndBattle);
			yield return new WaitForSeconds(3f);

			StopBattle();
		}

		private void SkipTurn()
		{
			isSkipTurn = true;
		}

		private void BattlePhaseInitialization()
		{
			uiManager.Battle.Messages.ShowCommenceBattle();
			uiManager.Battle.SkipTurn.onClick.AddListener(SkipTurn);
			uiManager.Battle.RunAway.onClick.AddListener(StopBattle);

			UpdateStates();

			cachedCharacter = characterManager.Party.CurrentCharacter;

			uiManager.Battle.SetBattle(CurrentBattle);
		}
		private void BattlePhaseCompletion()
		{
			LookAt(cachedCharacter);

			uiManager.Battle.Messages.HideTurnInforamtion();
			uiManager.Battle.SetBattle(null);
			uiManager.Battle.SkipTurn.onClick.RemoveAllListeners();
			uiManager.Battle.RunAway.onClick.RemoveAllListeners();


			UpdateStates();

			currentBattles.Remove(CurrentBattle);
			CurrentBattle = null;
		}

		private void LookAt(IEntity entity)
		{
			if(entity is Character character)
			{
				if (!characterManager.Party.SetCharacter(character))
				{
					cameraController.CameraToHome();
				}
			}
			else
			{
				cameraController.SetFollowTarget(entity.CameraPivot);
			}
		}

		private void UpdateStates()
		{
			IEntity initiator = CurrentBattle.CurrentTurn.initiator;
			bool isEndBattle = gameManager.CurrentGameState == GameState.EndBattle;

			CurrentBattle.entities.ForEach((x) =>
			{
				x.Freeze(!isEndBattle);

				if (characterManager.Party.Characters.Contains(x))
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

				x.Navigation.BattlePassive();
			});

			if(gameManager.CurrentGameState == GameState.Battle)
			{
				initiator.Freeze(false);

				initiator.Navigation.BattleActive();
			}
		}

		private void UpdateInitiatorTurn()
		{
			IEntity initiator = CurrentBattle.CurrentTurn.initiator;
			bool isMineTurn = characterManager.Party.Characters.Contains(initiator);

			LookAt(initiator);

			uiManager.Battle.Messages.ShowTurnInforamtion(isMineTurn ? "YOU TURN" : "ENEMY TURN");

			uiManager.Battle.SkipTurn.gameObject.SetActive(isMineTurn);
			uiManager.Battle.RunAway.gameObject.SetActive(isMineTurn);
		}

		private IEnumerator InitiatorTurn()
		{
			IEntity initiator = CurrentBattle.CurrentTurn.initiator;
			bool isMineTurn = characterManager.Party.Characters.Contains(initiator);

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

			if (!CurrentBattle.NextTurn())
			{
				if (!CurrentBattle.NextRound())
				{
					Debug.LogError("WIN!");
				}
				else
				{
					uiManager.Battle.Messages.ShowNewRound();
				}
			}
		}

		public class Battle
		{
			public UnityAction onBattleUpdated;

			public bool isShuffleAllRounds = true;

			public Round CurrentRound => rounds.First();
			public Turn CurrentTurn => CurrentRound.turns[0];//max 17 in one round + 1separator = 18

			public List<Round> rounds = new List<Round>();
			public List<IEntity> entities = new List<IEntity>();

			public int TurnCount
			{
				get
				{
					int result = 0;
					rounds.ForEach((x) =>
					{
						result += x.turns.Count;
					});

					return result;
				}
			}

			public void CreateStartRounds()
			{
				Round round = new Round();

				entities.ForEach((x) =>
				{
					round.turns.Add(new Turn()
					{
						initiator = x,
					});
				});

				rounds.Add(round);
				round.Shuffle();
				rounds.Add(isShuffleAllRounds ? round.Copy().Shuffle() : round.Copy());
			}
		

			public bool NextRound()
			{
				if(rounds.Count > 1)
				{
					rounds.RemoveAt(0);

					rounds.Add(isShuffleAllRounds ? rounds.First().Copy().Shuffle() : rounds.First().Copy());

					onBattleUpdated?.Invoke();

					return true;
				}

				return false;
			}

			public bool NextTurn()
			{
				if(CurrentRound.turns.Count > 1)
				{
					CurrentRound.turns.RemoveAt(0);

					onBattleUpdated?.Invoke();

					return true;
				}

				return false;
			}
		}
		public class Round : ICopyable<Round>
		{
			public List<Turn> turns = new List<Turn>();

			public Round Shuffle()
			{
				turns = turns.OrderBy((x) => Guid.NewGuid()).ToList();//initiative

				return this;
			}

			public Round Copy()
			{
				Round round = new Round()
				{
					turns = new List<Turn>(turns),
				};

				return round;
			}
		}
		public class Turn
		{
			public IEntity initiator;
		}
	}
}