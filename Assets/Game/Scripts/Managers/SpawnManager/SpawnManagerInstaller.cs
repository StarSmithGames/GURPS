using Zenject;

namespace Game.Systems.SpawnManager
{
	public class SpawnManagerInstaller : Installer<SpawnManagerInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<SpawnManager>().AsSingle().NonLazy();
		}
	}
}