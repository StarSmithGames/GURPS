using Zenject;
using UnityEngine;

namespace Game.Systems.NotificationSystem
{
	[CreateAssetMenu(fileName = "NotificationInstaller", menuName = "Installers/NotificationInstaller")]
	public class NotificationInstaller : ScriptableObjectInstaller<NotificationInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<NotificationSystem>().AsSingle();
		}
	}
}