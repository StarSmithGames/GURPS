using Cinemachine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class SceneInstaller : MonoInstaller
{
	[SerializeField] private CinemachineBrain brainCamera;
	[SerializeField] private UIManager uiManager;

	public override void InstallBindings()
	{
		Container.BindInstance(brainCamera);
		Container.BindInstance(uiManager);

		Container.BindInterfacesAndSelfTo<CharacterManager>().AsSingle();

		Container.BindInterfacesAndSelfTo<CameraVision>().AsSingle();
		Container.BindInterfacesAndSelfTo<CameraController>().AsSingle();
		Container.BindInterfacesAndSelfTo<PointClickInput>().AsSingle();

		Container.BindInterfacesAndSelfTo<UIHandlerTEST>().AsSingle();
	}
}
