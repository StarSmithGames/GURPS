using Game.Systems.CommandCenter;

using Zenject;

namespace Game.Managers.PartyManager
{
	public class PartyManagerInstaller : Installer<PartyManagerInstaller>
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<SignalLeaderPartyChanged>();
			Container.DeclareSignal<SignalPartyChanged>();

			Container.BindInterfacesAndSelfTo<PartyManager>().AsSingle();

			//command exectutor
			Container.BindInterfacesTo<PartyManagerCommandExecutor>().AsSingle().NonLazy();
		}
	}
}