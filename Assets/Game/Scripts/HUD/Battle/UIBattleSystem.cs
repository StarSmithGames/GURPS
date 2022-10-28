using Game.UI.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Game.UI.CanvasSystem;
using Game.Systems.BattleSystem;

namespace Game.HUD
{
	public class UIBattleSystem : WindowBase
	{
		[field: Space]
		[field: SerializeField] public WindowRoundQueue RoundQueue { get; private set; }
		[field: SerializeField] public UIMessages Messages { get; private set; }
		[field: Space]
		[field: SerializeField] public Button SkipTurn { get; private set; }
		[field: SerializeField] public Button RunAway { get; private set; }

		private BattleExecutor currentBattleExecutor;

		private SignalBus signalBus;
		private UISubCanvas subCanvas;

		[Inject]
		private void Construct(SignalBus signalBus, UISubCanvas subCanvas)
		{
			this.signalBus = signalBus;
			this.subCanvas = subCanvas;
		}

		private void Start()
		{
			SkipTurn.onClick.AddListener(OnSkipTurn);
			RunAway.onClick.AddListener(OnRunAway);

			signalBus?.Subscribe<SignalCurrentBattleExecutorChanged>(OnBattleExecutorChanged);

			subCanvas.WindowsRegistrator.Registrate(this);

			RoundQueue.Enable(false);
			Enable(false);
		}

		private void OnDestroy()
		{
			if (currentBattleExecutor != null)
			{
				currentBattleExecutor.onBattleStateChanged -= OnBattleStateChanged;
				currentBattleExecutor.onBattleOrderChanged -= OnBattleOrderChanged;

				currentBattleExecutor = null;
			}

			subCanvas.WindowsRegistrator.UnRegistrate(this);

			SkipTurn?.onClick.RemoveAllListeners();
			RunAway?.onClick.RemoveAllListeners();
		}

		private void OnBattleExecutorChanged(SignalCurrentBattleExecutorChanged signal)
		{
			if (currentBattleExecutor != null)
			{
				currentBattleExecutor.onBattleStateChanged -= OnBattleStateChanged;
				currentBattleExecutor.onBattleOrderChanged -= OnBattleOrderChanged;
				//currentBattleExecutor.Battle.onBattleUpdated -= OnBattleUpdated;
			}

			currentBattleExecutor = signal.currentBattleExecutor;

			if (currentBattleExecutor != null)
			{
				currentBattleExecutor.onBattleStateChanged += OnBattleStateChanged;
				currentBattleExecutor.onBattleOrderChanged += OnBattleOrderChanged;
			}
		}

		private void OnBattleStateChanged(BattleExecutorState oldState, BattleExecutorState newState)
		{
			if(newState == BattleExecutorState.PreBattle)
			{
				currentBattleExecutor.Battle.onBattleUpdated += onBattleUpdated;
				onBattleUpdated();

				if (!IsShowing)
				{
					Show();
					RoundQueue.Show();
				}
				Messages.ShowCommenceBattle();
			}
			else if (newState == BattleExecutorState.EndBattle)
			{
				Hide();
				RoundQueue.Hide();

				currentBattleExecutor.Battle.onBattleUpdated -= onBattleUpdated;
			}
		}

		private void OnBattleOrderChanged(BattleOrder order)
		{
			if(order == BattleOrder.Turn)
			{
				var isPlayerTurn = currentBattleExecutor.IsPlayerTurn;

				if (isPlayerTurn)
				{
					SkipTurn.interactable = true;
					RunAway.interactable = true;

					if (!IsShowing)
					{
						Show();
					}
				}
				else
				{
					Enable(false);
				}

				Messages.TurnInformation.SetText(isPlayerTurn ? "YOU TURN" : "ENEMY TURN", isPlayerTurn ? TurnInformationBackground.Player : TurnInformationBackground.Enemy).Show();
			}
		}

		private void onBattleUpdated()
		{
			if(currentBattleExecutor != null && currentBattleExecutor.Battle != null)
			{
				RoundQueue.UpdateTurns(currentBattleExecutor.Battle.FSM.Rounds);
			}
		}

		private void OnSkipTurn()
		{
			SkipTurn.interactable = false;

			currentBattleExecutor.SkipTurn();
		}

		private void OnRunAway()
		{
			RunAway.interactable = false;

			currentBattleExecutor.TerminateBattle();
		}
	}
}