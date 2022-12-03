using DG.Tweening;
using UnityEngine;

namespace Game.UI
{
	public class UIAnimationBlinkComponent : MonoBehaviour
	{
		public bool IsPlaying => blinkSequence?.IsPlaying() ?? false;

		[SerializeField] private CanvasGroup canvasGroup;

		private Sequence blinkSequence;

		public void Do(float speed, float start = 1, float end = 0)
		{
			blinkSequence = DOTween.Sequence();

			float t = 0;

			blinkSequence
				.AppendCallback(() =>
				{
					canvasGroup.alpha = Mathf.Lerp(start, end, Mathf.PingPong(t * speed, 1));

					t += Time.deltaTime;
				}).SetLoops(-1);
		}

		public void Kill(float alpha = 1)
		{
			if (blinkSequence != null)
			{
				if (blinkSequence.IsPlaying())
				{
					blinkSequence.Kill(true);
					blinkSequence = null;

					canvasGroup.alpha = alpha;
				}
			}
		}
	}
}