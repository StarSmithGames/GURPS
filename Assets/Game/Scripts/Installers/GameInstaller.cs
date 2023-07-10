using Cinemachine;
using Game.Managers.CharacterManager;
using Game.Managers.GameManager;
using Game.Managers.PartyManager;
using Game.Systems.SheetSystem;
using Game.Systems.SpawnManager;
using Game.UI.CanvasSystem;
using Zenject;

namespace Game
{
	public class GameInstaller : MonoInstaller<GameInstaller>
	{
		public UIGameCanvas gameCanvas;
		public CinemachineBrain brainCamera;

		public override void InstallBindings()
		{
			Container.BindInstance(gameCanvas);
			Container.BindInstance(brainCamera);

			gameCanvas.gameObject.DontDestroyOnLoad();
			brainCamera.gameObject.DontDestroyOnLoad();

			Container.BindInterfacesAndSelfTo<GamePipeline>().AsSingle().NonLazy();

			CharacterManagerInstaller.Install(Container);
			PartyManagerInstaller.Install(Container);
			SpawnManagerInstaller.Install(Container);
			SheetSystemInstaller.Install(Container);
		}
	}
}