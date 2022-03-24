using Game.Entities;
using Game.Managers.CharacterManager;
using Game.Managers.GameManager;

using System;
using System.Collections;
using System.Collections.Generic;
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

			Battle battle = new Battle()
			{
				entities = entities,
			};

			battle.CreateStartRounds();


			CurrentBattle = battle;
			currentBattles.Add(battle);

			asyncManager.StartCoroutine(BattleProcess());
		}

		public void StopBattle()
		{
			gameManager.ChangeState(GameState.Gameplay);
		}

		private bool isSkipTurn = false;

		private IEnumerator BattleProcess()
		{
			BattlePhaseInitialization();

			yield return new WaitForSeconds(3f);

			gameManager.ChangeState(GameState.Battle);

			while (true)
			{
				UpdateMarkers();

				var initiator = CurrentBattle.CurrentTurn.initiator;

				bool isMineTurn = false;

				if (characterManager.Party.Characters.Contains(initiator))
				{
					cameraController.SetFollowTarget(initiator.Transform);
					characterManager.Party.SetCharacter(initiator as Character);

					uiManager.Battle.Messages.ShowTurnInforamtion("YOU TURN");

					isMineTurn = true;
				}
				else
				{
					cameraController.SetFollowTarget(initiator.Transform);

					uiManager.Battle.Messages.ShowTurnInforamtion("ENEMY TURN");
				}

				uiManager.Battle.SkipTurn.gameObject.SetActive(isMineTurn);
				uiManager.Battle.RunAway.gameObject.SetActive(isMineTurn);

				if (isMineTurn)
				{
					initiator.Freeze(false);

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
						break;
					}
					else
					{
						uiManager.Battle.Messages.ShowNewRound();
					}
				}

				Debug.LogError("NextTurn!");

				yield return null;
			}

			yield return new WaitForSeconds(3f);

			BattlePhaseCompletion();
		}

		private void SkipTurn()
		{
			isSkipTurn = true;
		}

		private void RunAway()
		{
			BattlePhaseCompletion();
		}


		private Character cachedCharacter;

		private void BattlePhaseInitialization()
		{
			uiManager.Battle.Messages.ShowCommenceBattle();
			uiManager.Battle.SkipTurn.onClick = SkipTurn;
			uiManager.Battle.RunAway.onClick = RunAway;

			CurrentBattle.entities.ForEach((x) =>
			{
				x.Freeze(true);
			});

			cachedCharacter = characterManager.Party.CurrentCharacter;

			uiManager.Battle.SetBattle(CurrentBattle);
		}
		private void BattlePhaseCompletion()
		{
			cameraController.SetFollowTarget(cachedCharacter.Transform);
			characterManager.Party.SetCharacter(cachedCharacter);

			uiManager.Battle.Messages.HideTurnInforamtion();
			uiManager.Battle.SetBattle(null);

			CurrentBattle.entities.ForEach((x) =>
			{
				x.Freeze(false);
			});

			currentBattles.Remove(CurrentBattle);
			CurrentBattle = null;

			StopBattle();
		}

		private void UpdateMarkers()
		{
			CurrentBattle.entities.ForEach((x) =>
			{
				if (characterManager.Party.Characters.Contains(x))
				{
					if(x == CurrentBattle.CurrentTurn.initiator)
					{
						x.MarkerController.SetFollowMaterial(MaterialType.Leader);
					}
					else
					{
						x.MarkerController.SetFollowMaterial(MaterialType.Companion);
					}
				}
				else
				{
					x.MarkerController.SetFollowMaterial(MaterialType.Enemy);
				}

			});
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