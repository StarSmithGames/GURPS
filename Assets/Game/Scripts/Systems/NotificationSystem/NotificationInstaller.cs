using Zenject;
using UnityEngine;

namespace Game.Systems.NotificationSystem
{
	[CreateAssetMenu(fileName = "NotificationInstaller", menuName = "Installers/NotificationInstaller")]
	public class NotificationInstaller : ScriptableObjectInstaller<NotificationInstaller>
	{
		public UIJournalNotification notificationPrefab;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<NotificationSystem>().AsSingle();

			Container.BindFactory<UIJournalNotification, UIJournalNotification.Factory>()
					.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
					.FromComponentInNewPrefab(notificationPrefab));
		}
	}
}