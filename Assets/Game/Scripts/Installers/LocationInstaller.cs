using Cinemachine;

using Game.Systems.CameraSystem;
using Game.Systems.CombatDamageSystem;
using Game.Systems.SheetSystem.Skills;
using Game.UI.CanvasSystem;

using System.Collections.Generic;

using UnityEngine;
using Zenject;

namespace Game
{
	public class LocationInstaller : MonoInstaller
	{
		public UISubCanvas subCanvas;

		[Header("Cameras")]
		public List<CinemachineVirtualCamera> characterCamers = new List<CinemachineVirtualCamera>();

		public override void InstallBindings()
		{
			Container.BindInstance(subCanvas);
			BindCameras();

			CombatDamageSystemInstaller.Install(Container);
		}

		private void BindCameras()
		{
			Container.BindInstance(characterCamers).WithId("Camers");

			Container.BindInterfacesAndSelfTo<CameraVisionLocation>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<CameraController>().AsSingle().NonLazy();
		}
	}
}