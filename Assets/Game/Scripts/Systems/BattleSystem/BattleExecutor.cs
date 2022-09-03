using Game.Entities;
using Game.Systems.CameraSystem;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Zenject;
using Game.Systems.SheetSystem;
using Game.Entities.Models;
using Game.UI;
using Game.Managers.PartyManager;
using DG.Tweening;
using UnityEngine.Events;

namespace Game.Systems.BattleSystem
{
	public partial class BattleExecutor
	{
		/// <summary>
		/// Last state && New state
		/// </summary>
		public UnityAction<BattleExecutorState, BattleExecutorState> onBattleStateChanged;
		public UnityAction<BattleOrder> onBattleOrderChanged;

		public bool InitiatorCanAct { get; private set; }
		public bool IsPlayerTurn { get; private set; }

		public Battle Battle { get; private set; }
		public BattleExecutorState CurrentState { get; private set; }
		public BattleOrder CurrentOrder { get; private set; }
		public Turn CurrentTurn { get; private set; }
		public IBattlable CurrentInitiator { get; private set; }
		public List<IBattlable> Entities { get; private set; }

		public bool IsBattleProcess => battleCoroutine != null;
		private Coroutine battleCoroutine = null;

		private ICharacter cachedLeader;
		private bool isSkipTurn = false;
		private bool terminateBattle = false;

		private Settings settings;
		private AsyncManager asyncManager;
		private Battle.Factory battleFactory;
		private UISubCanvas subCanvas;
		private PartyManager partyManager;
		private CameraController cameraController;

		public BattleExecutor(
			Settings settings,
			AsyncManager asyncManager,
			Battle.Factory battleFactory,
			UISubCanvas subCanvas,
			PartyManager partyManager,
			CameraController cameraController)
		{
			this.settings = settings;
			this.asyncManager = asyncManager;
			this.battleFactory = battleFactory;
			this.subCanvas = subCanvas;
			this.partyManager = partyManager;
			this.cameraController = cameraController;
		}

		public void Initialize()
		{
			SetState(BattleExecutorState.Initialization);

			Entities = new List<IBattlable>(settings.entities);

			cachedLeader = partyManager.PlayerParty.LeaderParty;

			//Battle
			Battle = battleFactory.Create(Entities);
			Entities.ForEach((x) =>
			{
				x.JoinBattle(this);
			});

			Battle.onNextTurn += OnTurnChanged;

			InitiatorCanAct = true;//skip first InitiatorRecovery

			SetState(BattleExecutorState.PreBattle);
		}

		public void Dispose()
		{
			Battle.onNextTurn -= OnTurnChanged;

			Entities.ForEach((x) =>
			{
				x.LeaveBattle();
			});
		}

		public void Start()
		{
			if (!IsBattleProcess)
			{
				SetState(BattleExecutorState.Battle);

				battleCoroutine = asyncManager.StartCoroutine(BattleProcess());
			}

			UpdateStates();
		}

		public void Stop()
		{
			cameraController.LookAt(cachedLeader.Model as ICameraReporter);

			if (IsBattleProcess)
			{
				asyncManager.StopCoroutine(battleCoroutine);
				battleCoroutine = null;
			}
		}


		private IEnumerator BattleProcess()
		{
			while (!terminateBattle)
			{
				UpdateStates();
				//UpdateInitiatorTurn();

				if (!InitiatorCanAct)
				{
					yield return InitiatorRecovery();
				}

				yield return InitiatorTurn();
			}

			SetState(BattleExecutorState.EndBattle);

			yield return new WaitWhile(() => CurrentInitiator.InAction);
			yield return null;//? initiator stuck without frame

			Stop();
		}

		private IEnumerator InitiatorRecovery()
		{
			SetOrder(BattleOrder.Recovery);

			//statActions
			var statActions = (CurrentInitiator as ISheetable).Sheet.Stats.ActionPoints;
			bool isStatActionsReady = false;
			statActions.CurrentValue = statActions.MaxValue;
			isStatActionsReady = true;

			//statMove
			var statMove = (CurrentInitiator as ISheetable).Sheet.Stats.Move;
			bool isStatMoveReady = statMove.CurrentValue == statMove.MaxValue;
			if (!isStatMoveReady)
			{
				AnimateStat(statMove, statMove.CurrentValue, statMove.MaxValue, 0.25f, () => isStatMoveReady = true);
			}

			yield return new WaitUntil(() => isStatMoveReady && isStatActionsReady);
			InitiatorCanAct = true;
		}

		private IEnumerator InitiatorTurn()
		{
			SetOrder(BattleOrder.Turn);

			//Death
			//if ((CurrentInitiator as ISheetable).Sheet.Conditions.IsContains<Death>())
			//{
			//	isSkipTurn = true;
			//}

			if (!isSkipTurn)
			{
				if (IsPlayerTurn)
				{
					//Player Turn
					while (IsPlayerTurn && !isSkipTurn)
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
			}

			Battle.NextTurn();
		}


		private void UpdateStates()
		{
			var initiator = Battle.FSM.CurrentTurn.Initiator;
			bool isEndBattle = CurrentState == BattleExecutorState.EndBattle;

			Entities.ForEach((x) =>
			{
				if (x is ICharacterModel model)
				{
					if (!isEndBattle)
					{
						if (partyManager.PlayerParty.Contains(model.Character))
						{
							if (model == initiator)
							{
								model.Markers.SetFollowMaterial(MaterialType.Leader);
							}
							else
							{
								model.Markers.SetFollowMaterial(MaterialType.Companion);
							}
						}
						else
						{
							model.Markers.SetFollowMaterial(MaterialType.Enemy);
						}
					}

					model.Freeze(!isEndBattle);
				}
			});

			if (CurrentState == BattleExecutorState.Battle)
			{
				if (initiator is ICharacterModel model)
				{
					model.Freeze(false);
				}
			}
		}

		private void SetState(BattleExecutorState state)
		{
			var lastState = CurrentState;
			CurrentState = state;

			onBattleStateChanged?.Invoke(lastState, CurrentState);
		}

		private void SetOrder(BattleOrder order)
		{
			CurrentOrder = order;

			onBattleOrderChanged?.Invoke(CurrentOrder);
		}


		private void OnTurnChanged()
		{
			//UnSubscribtions
			if (CurrentInitiator != null)
			{
				if(CurrentInitiator is IPathfinderable pathfinderable)
				{
					pathfinderable.onDestinationChanged -= OnInitiatorDestinationChanged;
				}
			}

			CurrentTurn = Battle.FSM.CurrentTurn;
			CurrentInitiator = CurrentTurn.Initiator;

			cameraController.LookAt(CurrentInitiator as ICameraReporter);

			InitiatorCanAct = false;
			IsPlayerTurn = CurrentInitiator is ICharacterModel model ? partyManager.PlayerParty.Contains(model.Character) : false;

			//Subscribtions
			if (CurrentInitiator is IPathfinderable pathfinderable1)
			{
				pathfinderable1.onDestinationChanged += OnInitiatorDestinationChanged;
			}
		}

		private void OnInitiatorDestinationChanged()
		{
			if ((CurrentInitiator as IPathfinderable).IsHasTarget)
			{
				//if (!settings.isInfinityMoveStat)//required cheat
				{
					asyncManager.StartCoroutine(InitiatorSpendMove());
				}
			}
		}

		private void OnDied(IEntityModel entity)
		{
			//if (Entities.Contains(entity))
			//{
			//	foreach (var round in BattleFSM.Rounds)
			//	{
			//		for (int i = round.Turns.Count - 1; i >= 0; i--)
			//		{
			//			var turn = round.Turns[i];

			//			if (turn.Initiator == entity)
			//			{
			//				if (BattleFSM.CurrentTurn == turn)
			//				{
			//					NextTurn();
			//				}

			//				if (round.Remove(turn))
			//				{
			//					onBattleUpdated?.Invoke();
			//				}
			//			}
			//		}
			//	}

			//	Entities.Remove(entity as IBattlable);
			//	onEntitiesChanged?.Invoke();
			//}
		}


		public class Settings
		{
			public List<IBattlable> entities;
		}

		public class Data
		{
			//cached leader.//how blyat
		}

		public class Factory : PlaceholderFactory<Settings, BattleExecutor> { }
	}

	//Skip
	partial class BattleExecutor
	{
		//public bool IsSkipProcess => skipCoroutine != null;
		//private Coroutine skipCoroutine = null;

		public void SkipTurn()
		{
			isSkipTurn = true;
		}

		public void TerminateBattle()
		{
			SetState(BattleExecutorState.EndBattle);
			Stop();
			Dispose();
		}
	}

	//Sheet
	partial class BattleExecutor
	{
		private IEnumerator InitiatorSpendMove()
		{
			IStat stat = (CurrentInitiator as ISheetable).Sheet.Stats.Move;

			NavigationController navigation = (CurrentInitiator as IPathfinderable).Navigation;

			float from = stat.CurrentValue;
			float to = stat.CurrentValue - navigation.CurrentNavMeshPathDistance;

			while (navigation.NavMeshAgent.IsReachesDestination())
			{
				stat.CurrentValue = Mathf.Lerp(from, to, navigation.NavMeshInvertedPercentRemainingDistance);

				yield return null;
			}

			stat.CurrentValue = to;
			if (stat.CurrentValue < 0)
			{
				Debug.LogError("ERROR STAT");
			}
		}

		private void AnimateStat(IStatBar stat, float from, float to, float t, UnityAction callback = null)
		{
			DOTween.To(() => from, (x) => stat.CurrentValue = x, to, t).OnComplete(() => callback?.Invoke());
		}
	}
}