using Cinemachine;
using Game.Systems.CameraSystem;
using System.Collections.Generic;
using Zenject;

namespace Game
{
	public class MapInstaller : MonoInstaller
	{
		public CinemachineBrain brainCamera;
		public List<CinemachineVirtualCamera> characterCamers = new List<CinemachineVirtualCamera>();

		public override void InstallBindings()
		{
			BindCameras();
		}

		private void BindCameras()
		{
			Container.BindInstance(brainCamera);
			Container.BindInstance(characterCamers).WithId("Camers");

			Container.BindInterfacesTo<CameraVisionMap>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<CameraController>().AsSingle().NonLazy();
		}
	}
}