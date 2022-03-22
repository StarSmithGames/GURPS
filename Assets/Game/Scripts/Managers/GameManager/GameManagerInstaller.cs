using Zenject;

namespace Game.Managers.GameManager
{
	public class GameManagerInstaller : Installer<GameManagerInstaller>
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<SignalGameStateChanged>();

			Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
		}
	}
}