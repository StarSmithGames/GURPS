using Zenject;

namespace Game.Managers.GameManager
{
	public class GameManagerInstaller : Installer<GameManagerInstaller>
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<SignalGameStateChanged>();
			Container.DeclareSignal<SignalGameLocationChanged>();

			Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
		}
	}
}