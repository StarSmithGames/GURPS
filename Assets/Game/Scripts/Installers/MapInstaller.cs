using Game.Systems.CameraSystem;
using Game.UI.CanvasSystem;

using Zenject;

namespace Game
{
	public class MapInstaller : MonoInstaller
	{
		public UISubCanvas subCanvas;

		public override void InstallBindings()
		{
			Container.BindInstance(subCanvas);

			//BindCameras();
		}

		//private void BindCameras()
		//{
		//	Container.BindInstance(brainCamera);
		//	Container.BindInstance(characterCamers).WithId("Camers");

		//	Container.BindInterfacesTo<CameraVisionMap>().AsSingle().NonLazy();
		//	Container.BindInterfacesAndSelfTo<CameraController>().AsSingle().NonLazy();
		//}
	}
}