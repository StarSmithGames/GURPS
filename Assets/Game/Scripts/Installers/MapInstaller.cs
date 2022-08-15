using Cinemachine;

using Game.Entities;
using Game.Managers.StorageManager;
using Game.Systems.CameraSystem;

using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class MapInstaller : MonoInstaller
{
	public CinemachineBrain brainCamera;
	public List<CinemachineVirtualCamera> characterCamers = new List<CinemachineVirtualCamera>();

	public override void InstallBindings()
	{
		Container.BindInstance(brainCamera);
		Container.BindInstance(characterCamers).WithId("Camers");

		Container.BindInterfacesTo<CameraVisionMap>().AsSingle().NonLazy();
		Container.BindInterfacesAndSelfTo<CameraController>().AsSingle().NonLazy();
	}
}