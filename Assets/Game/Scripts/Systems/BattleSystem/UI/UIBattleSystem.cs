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

		private Battle currentBattle;

		private UISubCanvas subCanvas;

		[Inject]
		private void Construct(UISubCanvas subCanvas)
		{
			this.subCanvas = subCanvas;
		}

		private void Start()
		{
			subCanvas.WindowsRegistrator.Registrate(this);

			Enable(false);
		}

		private void OnDestroy()
		{
			subCanvas.WindowsRegistrator.UnRegistrate(this);
		}

		public void SetBattle(Battle battle)
		{
			if (currentBattle != null)
			{
				currentBattle.onBattleUpdated -= OnBattleUpdated;
			}

			currentBattle = battle;

			if (currentBattle != null)
			{
				currentBattle.onBattleUpdated += OnBattleUpdated;
			}
		}

		public void Show(UnityAction callback = null)
		{
			IsShowing = true;

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
					IsShowing = false;
					callback?.Invoke();
				});
		}

		public void Enable(bool trigger)
		{
			RoundQueue.Enable(trigger);
			CanvasGroup.Enable(trigger);
			IsShowing = trigger;
		}


		private void OnBattleUpdated()
		{
			if(currentBattle != null)
			{
				RoundQueue.UpdateTurns(currentBattle.BattleFSM.Rounds);
			}
		}
	}
}