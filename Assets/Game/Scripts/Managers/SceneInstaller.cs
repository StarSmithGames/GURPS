using Cinemachine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class SceneInstaller : MonoInstaller
{
	[SerializeField] private CinemachineBrain brainCamera;

	public override void InstallBindings()
	{
		Container.BindInstance(brainCamera);

		Container.BindInterfacesAndSelfTo<CameraVision>().AsSingle();

		Container.BindInterfacesAndSelfTo<CameraController>().AsSingle();
	}
}