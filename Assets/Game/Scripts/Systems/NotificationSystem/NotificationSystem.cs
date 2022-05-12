using UnityEngine;
using DG.Tweening;

namespace Game.Systems.NotificationSystem
{
	public class NotificationSystem
	{
		private JournalNotification journalNotification;

		public NotificationSystem(UIManager uiManager)
		{
			journalNotification = new JournalNotification(uiManager.JournalNotification);
		}

		public void PushJournal(string title)
		{
			journalNotification.Push(title);
		}
	}

	public class JournalNotification
	{
		private UIJournalNotification notification;

		public JournalNotification(UIJournalNotification ui)
		{
			notification = ui;

			notification.Enable(false);
		}

		public void Push(string title)
		{
			if (notification.IsShowing) return;

			Vector3 endPosition = notification.transform.position;
			Vector2 startPosition = (notification.transform as RectTransform).anchoredPosition - new Vector2(0, 50);

			notification.Title.text = title;
			notification.CanvasGroup.alpha = 0f;
			notification.Enable(true);
			(notification.transform as RectTransform).anchoredPosition = startPosition;

			Sequence sequence = DOTween.Sequence();

			sequence
				.Append(notification.transform.DOMove(endPosition, 0.25f))
				.Join(notification.CanvasGroup.DOFade(1f, 0.15f))
				.AppendInterval(3f)
				.Append(notification.CanvasGroup.DOFade(0f, 0.3f))
				.AppendCallback(() =>
				{
					notification.Enable(false);
				});
		}
	}
}