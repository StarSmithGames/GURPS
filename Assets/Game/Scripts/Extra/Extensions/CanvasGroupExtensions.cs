using UnityEngine;

using DG.Tweening;
using UnityEngine.Events;

public static class CanvasGroupExtensions
{
	public static void Enable(this CanvasGroup canvasGroup, bool trigger, bool withAlpha = true)
	{
		if (withAlpha)
		{
			canvasGroup.alpha = trigger ? 1f : 0f;
		}
		canvasGroup.blocksRaycasts = trigger;
		canvasGroup.interactable = trigger;
	}

	public static void DoFadeIn(this CanvasGroup canvasGroup, UnityAction callback = null)
	{
		Sequence sequence = DOTween.Sequence();

		canvasGroup.Enable(true, false);

		sequence
			.Append(canvasGroup.DOFade(1f, 0.2f))
			.AppendCallback(() => callback?.Invoke());
	}

	public static void DoFadeOut(this CanvasGroup canvasGroup, UnityAction callback = null)
	{
		Sequence sequence = DOTween.Sequence();

		sequence
			.Append(canvasGroup.DOFade(0f, 0.15f))
			.AppendCallback(() =>
			{
				canvasGroup.Enable(false);
				callback?.Invoke();
			});
	}
}