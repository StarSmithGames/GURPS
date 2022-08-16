using DG.Tweening;

using Game.UI;

using UnityEngine;

using Zenject;

namespace Game.Systems.NotificationSystem
{
	public class UIJournalNotification : UIPollable
	{
		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }

		[field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Title { get; private set; }
	
		public void Push(string title)
		{
			Vector3 endPosition = transform.position;
			Vector2 startPosition = (transform as RectTransform).anchoredPosition - new Vector2(0, 50);

			Title.text = title;
			CanvasGroup.alpha = 0f;

			Enable(true);
			(transform as RectTransform).anchoredPosition = startPosition;

			Sequence sequence = DOTween.Sequence();

			sequence
				.Append(transform.DOMove(endPosition, 0.25f))
				.Join(CanvasGroup.DOFade(1f, 0.15f))
				.AppendInterval(3f)
				.Append(CanvasGroup.DOFade(0f, 0.3f))
				.AppendCallback(() =>
				{
					Enable(false);
				});
		}

		public class Factory : PlaceholderFactory<UIJournalNotification> { }
	}
}