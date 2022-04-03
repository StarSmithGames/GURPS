using Cinemachine;

using Game.Systems.BattleSystem;
using Game.Systems.CameraSystem;

using System.Collections.Generic;

using UnityEngine;

using Zenject;

public class SceneInstaller : MonoInstaller
{
	[SerializeField] private CinemachineBrain brainCamera;
	[SerializeField] private List<CinemachineVirtualCamera> characterCamers= new List<CinemachineVirtualCamera>();

	public override void InstallBindings()
	{
		BattleSystemInstaller.Install(Container);

		Container.BindInstance(brainCamera);
		Container.BindInstance(characterCamers).WithId("CharacterCamers");

		Container.BindInterfacesAndSelfTo<CameraVision>().AsSingle();

		Container.BindInterfacesAndSelfTo<CameraController>().AsSingle();
	}
}