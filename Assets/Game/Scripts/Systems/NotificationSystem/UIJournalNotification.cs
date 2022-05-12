using UnityEngine;

namespace Game.Systems.NotificationSystem
{
	public class UIJournalNotification : UIBase
	{
		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }

		[field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Title { get; private set; }
	}
}