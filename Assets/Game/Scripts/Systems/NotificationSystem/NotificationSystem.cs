using Game.UI.CanvasSystem;

namespace Game.Systems.NotificationSystem
{
	public class NotificationSystem
	{
		private UISubCanvas gameCanvas;
		private UIJournalNotification.Factory journalNotificationFactory;

		public NotificationSystem(UIGameCanvas gameCanvas, UIJournalNotification.Factory journalNotificationFactory)
		{
			this.gameCanvas = gameCanvas;
			this.journalNotificationFactory = journalNotificationFactory;
		}

		public void PushJournalNotification(string title)
		{
			var notification = journalNotificationFactory.Create();
			notification.transform.SetParent(gameCanvas.transform);
			notification.Push(title);
		}
	}
}