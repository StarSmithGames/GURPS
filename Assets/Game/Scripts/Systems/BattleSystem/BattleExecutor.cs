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

namespace Game.Systems.BattleSystem
{
	public class BattleExecutor
	{
		public bool IsBattleProcess => battleCoroutine != null;
		private Coroutine battleCoroutine = null;

		public IBattlable CurrentInitiator { get; private set; }
		public Turn CurrentTurn { get; private set; }

		private ICharacter cachedLeader;
		private bool isSkipTurn = false;
		private bool terminateBattle = false;

		private Battle battle;
		private UIBattleSystem battleSystemUI;

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
			battle = battleFactory.Create(settings.entities);

			battle.onNextTurn += OnTurnChanged;
			battle.onNextRound += OnRoundChanged;

			battle.Initialization();


			cachedLeader = partyManager.PlayerParty.LeaderParty;

			battleSystemUI = subCanvas.WindowsRegistrator.GetAs<UIBattleSystem>();
			battleSystemUI.SetBattle(battle);

			battleSystemUI.Messages.ShowCommenceBattle();
			//uiManager.Battle.SkipTurn.ButtonPointer.onClick.AddListener(StartSkipTurn);
			//uiManager.Battle.RunAway.ButtonPointer.onClick.AddListener(StopBattle);

			battle.SetState(BattleState.PreBattle);

			battleSystemUI.Show(() => battle.SetState(BattleState.Battle));
		}

		public void Dispose()
		{
			battle.onNextTurn -= OnTurnChanged;
			battle.onNextRound -= OnRoundChanged;

			battle.Dispose();
		}

		public void Start()
		{
			if (!IsBattleProcess)
			{
				battleCoroutine = asyncManager.StartCoroutine(BattleProcess());
			}

			UpdateStates();
		}

		public void Stop()
		{
			cameraController.LookAt(cachedLeader.Model);

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

				yield return InitiatorTurn();
			}

			battle.SetState(BattleState.EndBattle);

			yield return new WaitWhile(() => CurrentInitiator.InAction);
			yield return null;//? initiator stuck without frame

			Stop();
		}

		private IEnumerator InitiatorTurn()
		{
			//Death
			//if ((CurrentInitiator as ISheetable).Sheet.Conditions.IsContains<Death>())
			//{
			//	isSkipTurn = true;
			//}

			if (!isSkipTurn)
			{
				bool isMineTurn = CurrentInitiator is ICharacterModel model ? partyManager.PlayerParty.Contains(model.Character) : false;

				if (isMineTurn)
				{
					//Player Turn
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
			}

			battle.NextTurn();
		}


		private void UpdateStates()
		{
			var initiator = battle.BattleFSM.CurrentTurn.Initiator;
			bool isEndBattle = battle.CurrentState == BattleState.EndBattle;

			battle.Entities.ForEach((x) =>
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

			if (battle.CurrentState == BattleState.Battle)
			{
				if (initiator is ICharacterModel model)
				{
					model.Freeze(false);
				}
			}
		}

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
		}


		private void OnTurnChanged()
		{
			if (CurrentInitiator != null)
			{
				if(CurrentInitiator is IPathfinderable pathfinderable)
				{
					pathfinderable.onDestinationChanged -= OnInitiatorDestinationChanged;
				}
				(CurrentInitiator as ISheetable).Sheet.Stats.ActionPoints.onStatChanged -= OnInitiatorActionPointsChanged;
			}
			CurrentTurn = battle.BattleFSM.CurrentTurn;
			CurrentInitiator = CurrentTurn.Initiator;

			//InitiatorRecoveActionsPoints();
			//InitiatorRecoveMove();

			if (CurrentInitiator is IPathfinderable pathfinderable1)
			{
				pathfinderable1.onDestinationChanged += OnInitiatorDestinationChanged;
			}
			(CurrentInitiator as ISheetable).Sheet.Stats.ActionPoints.onStatChanged += OnInitiatorActionPointsChanged;
		}

		private void OnRoundChanged()
		{
			battleSystemUI.Messages.ShowNewRound();
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

		private void OnInitiatorActionPointsChanged()
		{
			//if (settings.isInfinityActionStat)
			//{
			//	var stat = (CurrentInitiator as ISheetable).Sheet.Stats.ActionPoints;
			//	if (stat.CurrentValue != stat.MaxValue)
			//	{
			//		stat.CurrentValue = stat.MaxValue;
			//	}
			//}
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

		public class Factory : PlaceholderFactory<Settings, BattleExecutor> { }
	}


	//partial class BattleSystem
	//{
	//	public bool IsSkipProcess => skipCoroutine != null;
	//	private Coroutine skipCoroutine = null;

	//	private IEnumerator SkipTurn()
	//	{
	//		yield return null;//WaitInitiatorAction();

	//		//isSkipTurn = true;

	//		skipCoroutine = null;
	//	}

	//	private void StartSkipTurn()
	//	{
	//		//if (IsSkipProcess) return;

	//		//skipCoroutine = asyncManager.StartCoroutine(SkipTurn());
	//	}
	//}
}