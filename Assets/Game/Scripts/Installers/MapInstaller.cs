using Cinemachine;
using Game.Systems.CameraSystem;
using Game.UI;

using System.Collections.Generic;

using UnityEngine;

using Zenject;

namespace Game
{
	public class MapInstaller : MonoInstaller
	{
		public UISubCanvas subCanvas;
		[Header("Cameras")]
		public CinemachineBrain brainCamera;
		public List<CinemachineVirtualCamera> characterCamers = new List<CinemachineVirtualCamera>();

		public override void InstallBindings()
		{
			Container.BindInstance(subCanvas);

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