using Cinemachine;

using Game.Systems.BattleSystem;

using UnityEngine;

using Zenject;

public class SceneInstaller : MonoInstaller
{
	[SerializeField] private CinemachineBrain brainCamera;

	public override void InstallBindings()
	{
		BattleSystemInstaller.Install(Container);

		Container.BindInstance(brainCamera);

		Container.BindInterfacesAndSelfTo<CameraVision>().AsSingle();

		Container.BindInterfacesAndSelfTo<CameraController>().AsSingle();
	}
}