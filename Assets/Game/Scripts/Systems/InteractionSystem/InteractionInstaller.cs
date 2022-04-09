using Zenject;

namespace Game.Systems.InteractionSystem
{
	public class InteractionInstaller : Installer<InteractionInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<InteractionHandler>().AsSingle();
		}
	}
}