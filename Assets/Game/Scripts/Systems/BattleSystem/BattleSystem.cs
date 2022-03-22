using Game.Entities;
using Game.Managers.CharacterManager;
using Game.Managers.GameManager;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

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

			battle.CreateRound();
			battle.ShuffleRound();

			CurrentBattle = battle;
			currentBattles.Add(battle);

			asyncManager.StartCoroutine(BattleProcess());
		}

		public void StopBattle()
		{
			gameManager.ChangeState(GameState.Gameplay);
		}

		private IEnumerator BattleProcess()
		{
			uiManager.Battle.ShowCommenceBattle();

			CurrentBattle.entities.ForEach((x) =>
			{
				x.Freeze();
			});

			uiManager.Battle.SetBattle(CurrentBattle);

			yield return new WaitForSeconds(3f);

			var cachedCharacter = characterManager.Party.CurrentCharacter;
			var initiator = CurrentBattle.rounds.First().turns[0].initiator;

			if (characterManager.Party.Characters.Contains(initiator))
			{
				characterManager.Party.SetCharacter(initiator as Character);
			}
			else
			{
				cameraController.SetFollowTarget(initiator.Transform);
			}

			yield return new WaitForSeconds(3f);

			characterManager.Party.SetCharacter(cachedCharacter);


			uiManager.Battle.SetBattle(null);

			CurrentBattle.entities.ForEach((x) =>
			{
				x.UnFreeze();
			});

			currentBattles.Remove(CurrentBattle);
			CurrentBattle = null;

			StopBattle();
		}

		public class Battle
		{
			public Round CurrentRound => rounds.First();

			public List<Round> rounds = new List<Round>();
			public List<IEntity> entities = new List<IEntity>();

			public void CreateRound()
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
			}

			public void ShuffleRound()
			{
				CurrentRound.turns = CurrentRound.turns.OrderBy((x) => Guid.NewGuid()).ToList();//initiative
			}
		}
		public class Round
		{
			public List<Turn> turns = new List<Turn>();
		}
		public class Turn
		{
			public IEntity initiator;
		}
	}
}