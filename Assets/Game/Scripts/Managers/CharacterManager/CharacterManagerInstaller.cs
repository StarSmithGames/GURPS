using Zenject;

namespace Game.Managers.CharacterManager
{
	public class CharacterManagerInstaller : Installer<CharacterManagerInstaller>
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<SignalLeaderPartyChanged>();

			Container.BindInterfacesAndSelfTo<CharacterManager>().AsSingle();
		}
	}
}