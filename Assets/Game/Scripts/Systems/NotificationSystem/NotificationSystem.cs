using UnityEngine;
using DG.Tweening;
using Game.UI;

namespace Game.Systems.NotificationSystem
{
	public class NotificationSystem
	{
		private UISubCanvas subCanvas;
		private UIJournalNotification.Factory journalNotificationFactory;

		public NotificationSystem(UISubCanvas subCanvas, UIJournalNotification.Factory journalNotificationFactory)
		{
			this.subCanvas = subCanvas;
			this.journalNotificationFactory = journalNotificationFactory;
		}

		public void PushJournalNotification(string title)
		{
			var notification = journalNotificationFactory.Create();
			notification.transform.SetParent(subCanvas.transform);
			notification.Push(title);
		}
	}
}