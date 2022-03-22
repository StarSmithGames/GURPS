using Game.Entities;
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
		private List<Battle> currentBattles = new List<Battle>();

		private GameManager gameManager;
		private UIManager uiManager;
		private UITurn.Factory turnFactory;

		public BattleSystem(GameManager gameManager, UIManager uiManager, UITurn.Factory turnFactory)
		{
			this.gameManager = gameManager;
			this.uiManager = uiManager;
			this.turnFactory = turnFactory;
		}

		public void StartBattle(List<IEntity> characters, IEntity enemy)
		{
			//gameManager.ChangeState(GameState.Battle);

			//currentBattles.Add(new Battle(characters, enemy));
		}

		public void EndBattle()
		{
			gameManager.ChangeState(GameState.Gameplay);
		}

		public class Battle
		{
			public List<Round> rounds = new List<Round>();

			private List<IEntity> entities;

			public Battle(List<IEntity> characters, IEntity enemy)
			{
				entities = new List<IEntity>(characters);
				entities.Add(enemy);

				StartRound();
			}

			private void StartRound()
			{
				Round round = new Round();

				entities.ForEach((x) =>
				{
					round.turns.Add(new Turn()
					{
						initiator = x,
					});
				});

				round.turns = round.turns.OrderBy((x) => Guid.NewGuid()).ToList();//initiative

				rounds.Add(round);
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