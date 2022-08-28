using Game.UI;
using Game.UI.Windows;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using DG.Tweening;

using Zenject;

namespace Game.Systems.BattleSystem
{
	public class UIBattleSystem : MonoBehaviour, IWindow
	{
		public bool IsShowing { get; private set; }

		[field: SerializeField] public WindowRoundQueue RoundQueue { get; private set; }
		[field: SerializeField] public UIMessages Messages { get; private set; }
		[field: Space]
		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
		[field: SerializeField] public Button SkipTurn { get; private set; }
		[field: SerializeField] public Button RunAway { get; private set; }

		private BattleExecutor battleExecutor;

		private UISubCanvas subCanvas;

		[Inject]
		private void Construct(UISubCanvas subCanvas)
		{
			this.subCanvas = subCanvas;
		}

		private void Start()
		{
			SkipTurn.onClick.AddListener(OnSkipTurn);
			RunAway.onClick.AddListener(OnRunAway);

			subCanvas.WindowsRegistrator.Registrate(this);

			RoundQueue.Enable(false);
			Enable(false);
		}

		private void OnDestroy()
		{
			subCanvas.WindowsRegistrator.UnRegistrate(this);

			SkipTurn?.onClick.RemoveAllListeners();
			RunAway?.onClick.RemoveAllListeners();
		}

		public void SetBattleExecutor(BattleExecutor battleExecutor)
		{
			if (battleExecutor != null)
			{
				battleExecutor.Battle.onBattleUpdated -= OnBattleUpdated;
			}

			this.battleExecutor = battleExecutor;

			if (battleExecutor != null)
			{
				battleExecutor.Battle.onBattleUpdated += OnBattleUpdated;
			}
		}

		public void Show(UnityAction callback = null)
		{
			IsShowing = true;
			CanvasGroup.Enable(true, false);

			Sequence sequence = DOTween.Sequence();

			sequence
				.Append(CanvasGroup.DOFade(1f, 0.25f))
				.AppendCallback(() => RoundQueue.Show())
				.OnComplete(() => callback?.Invoke());
		}

		public void Hide(UnityAction callback = null)
		{
			Sequence sequence = DOTween.Sequence();

			sequence
				.Append(CanvasGroup.DOFade(0f, 0.2f))
				.OnComplete(() =>
				{
					CanvasGroup.Enable(true);
					IsShowing = false;
					callback?.Invoke();
				});
		}

		public void Enable(bool trigger)
		{
			CanvasGroup.Enable(trigger);
			IsShowing = trigger;
		}


		private void OnBattleUpdated()
		{
			if(battleExecutor != null)
			{
				RoundQueue.UpdateTurns(battleExecutor.Battle.FSM.Rounds);
			}

			SkipTurn.interactable = true;
			RunAway.interactable = true;
		}

		private void OnSkipTurn()
		{
			SkipTurn.interactable = false;

			battleExecutor.SkipTurn();
		}

		private void OnRunAway()
		{
			RunAway.interactable = false;

		}
	}
}