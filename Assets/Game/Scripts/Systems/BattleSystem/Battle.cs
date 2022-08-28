using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.Events;

using Zenject;

namespace Game.Systems.BattleSystem
{
	public class Battle
	{
		public UnityAction onBattleUpdated;

		public UnityAction onBattleStateChanged;

		public UnityAction onVictory;
		public UnityAction onNextRound;
		public UnityAction onNextTurn;

		public BattleState CurrentState { get; private set; }
		public BattleFSM FSM { get; private set; }

		public Battle(List<IBattlable> entities)
		{
			FSM = new BattleFSM();
			
			List<Turn> turns = new List<Turn>();
			entities.ForEach((x) => turns.Add(new Turn(x)));

			Round round = new Round(turns);
			FSM.Rounds.Add(round);
			FSM.Rounds.Add(round.Copy());
		}

		public void NextTurn()
		{
			if (!FSM.NextTurn())
			{
				if (!FSM.NextRound())
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

		public class Factory : PlaceholderFactory<List<IBattlable>, Battle> { }
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
}