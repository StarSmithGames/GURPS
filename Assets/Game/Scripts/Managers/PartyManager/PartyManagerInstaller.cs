using Zenject;

namespace Game.Managers.PartyManager
{
	public class PartyManagerInstaller : Installer<PartyManagerInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<PartyManager>().AsSingle();
		}
	}
}