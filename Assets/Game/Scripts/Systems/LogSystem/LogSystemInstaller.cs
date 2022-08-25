using UnityEngine;

using Zenject;

namespace Game.Systems.LogSystem
{
	[CreateAssetMenu(menuName = "Installers/LogSystemInstaller", fileName = "LogSystemInstaller")]
	public class LogSystemInstaller : ScriptableObjectInstaller<LogSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<GameLog>().AsSingle();
		}
	}
}