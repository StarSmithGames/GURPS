using Cinemachine;

using Game.Systems.BattleSystem;
using Game.Systems.CameraSystem;
using Game.Systems.ContextMenu;
using Game.Systems.InventorySystem;
using Game.UI;

using System.Collections.Generic;

using UnityEngine;

using Zenject;

namespace Game
{
	public class LocationInstaller : MonoInstaller
	{
		public UISubCanvas subCanvas;
		[Header("Cameras")]
		public CinemachineBrain brainCamera;
		public List<CinemachineVirtualCamera> characterCamers = new List<CinemachineVirtualCamera>();

		public override void InstallBindings()
		{
			Container.BindInstance(subCanvas);

			BattleSystemInstaller.Install(Container);

			BindCameras();
		}

		private void BindCameras()
		{
			Container.BindInstance(brainCamera);
			Container.BindInstance(characterCamers).WithId("Camers");

			Container.BindInterfacesTo<CameraVisionLocation>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<CameraController>().AsSingle().NonLazy();
		}
	}
}