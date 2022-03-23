using DG.Tweening;

using UnityEngine;

namespace Game.Systems.BattleSystem
{
	public class UIMessages : MonoBehaviour
	{
		[field: SerializeField] public WindowInformer CommenceBattle { get; private set; }
		[field: SerializeField] public WindowInformer NewRound { get; private set; }
		[field: SerializeField] public WindowInformer SurpiseEnemy { get; private set; }
		[field: SerializeField] public WindowInformer TurnInforamtion { get; private set; }

		public void ShowCommenceBattle()
		{
			Sequence sequence = DOTween.Sequence();

			CommenceBattle.CanvasGroup.alpha = 0;

			sequence
				.AppendCallback(() => CommenceBattle.gameObject.SetActive(true))
				.Append(CommenceBattle.CanvasGroup.DOFade(1, 0.25f))
				.AppendInterval(1.5f)
				.Append(CommenceBattle.CanvasGroup.DOFade(0, 0.1f))
				.AppendCallback(() => CommenceBattle.gameObject.SetActive(false));
		}

		public void ShowNewRound()
		{
			Sequence sequence = DOTween.Sequence();

			NewRound.CanvasGroup.alpha = 0;

			sequence
				.AppendCallback(() => NewRound.gameObject.SetActive(true))
				.Append(NewRound.CanvasGroup.DOFade(1, 0.25f))
				.AppendInterval(1.5f)
				.Append(NewRound.CanvasGroup.DOFade(0, 0.1f))
				.AppendCallback(() => NewRound.gameObject.SetActive(false));
		}


		public void ShowTurnInforamtion(string text)
		{
			if (!TurnInforamtion.gameObject.activeSelf)
			{
				TurnInforamtion.CanvasGroup.alpha = 0;
				TurnInforamtion.gameObject.SetActive(true);
			}

			Sequence sequence = DOTween.Sequence();

			sequence
				.Append(TurnInforamtion.CanvasGroup.DOFade(0, 0.25f))
				.AppendCallback(() => TurnInforamtion.SetText(text))
				.Append(TurnInforamtion.CanvasGroup.DOFade(1, 0.25f));
		}

		public void HideTurnInforamtion()
		{
			Sequence sequence = DOTween.Sequence();

			sequence
				.Append(TurnInforamtion.CanvasGroup.DOFade(0, 0.25f))
				.AppendCallback(() => TurnInforamtion.gameObject.SetActive(false))
			;
		}
	}
}