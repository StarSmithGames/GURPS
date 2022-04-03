using Game.Systems.SheetSystem;

using UnityEngine;

using Zenject;

namespace Game.Systems.BattleSystem
{
	public class UIBattle : MonoBehaviour
	{
		[field: SerializeField] public UIRoundQueue RoundQueue { get; private set; }
		[field: SerializeField] public UIEntityInformation EntityInformation { get; private set; }
		[field: SerializeField] public UIMessages Messages { get; private set; }
		[field: Space]
		[field: SerializeField] public UIBar EnergyBar { get; private set; }
		[field: SerializeField] public UIButton SkipTurn { get; private set; }
		[field: SerializeField] public UIButton RunAway { get; private set; }

		private Battle currentBattle;

		private SignalBus signalBus;

		[Inject]
		private void Construct(SignalBus signalBus)
		{
			this.signalBus = signalBus;
		}

		private void OnDestroy()
		{
			signalBus?.Unsubscribe<SignalCurrentBattleChanged>(OnBattleChanged);
		}

		private void Start()
		{
			signalBus?.Subscribe<SignalCurrentBattleChanged>(OnBattleChanged);
		}

		public void SetSheet(ISheet sheet)
		{
			EntityInformation.SetSheet(sheet);
			EntityInformation.gameObject.SetActive(sheet != null);
		}

		private void OnBattleUpdated()
		{
			if(currentBattle != null)
			{
				RoundQueue.UpdateTurns(currentBattle.BattleFSM.Rounds);
			}
		}

		private void OnBattleStateChanged()
		{
			switch (currentBattle.CurrentState)
			{
				case BattleState.PreBattle:
				{
					RoundQueue.UpdateTurns(currentBattle.BattleFSM.Rounds);
					RoundQueue.gameObject.SetActive(true);
					break;
				}
				case BattleState.Battle:
				{
					break;
				}
				case BattleState.EndBattle:
				{
					RoundQueue.gameObject.SetActive(false);
					SkipTurn.gameObject.SetActive(false);
					RunAway.gameObject.SetActive(false);
					break;
				}
			}
		}


		private void OnBattleChanged(SignalCurrentBattleChanged signal)
		{

			if (currentBattle != null)
			{
				currentBattle.onBattleUpdated -= OnBattleUpdated;
				currentBattle.onBattleStateChanged -= OnBattleStateChanged;
			}

			currentBattle = signal.currentBattle;

			if (currentBattle != null)
			{
				currentBattle.onBattleUpdated += OnBattleUpdated;
				currentBattle.onBattleStateChanged += OnBattleStateChanged;
			}
		}
	}
}