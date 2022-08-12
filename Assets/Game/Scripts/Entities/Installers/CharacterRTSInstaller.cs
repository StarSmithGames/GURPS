using EPOOutline;

using Game.Entities;
using Game.Systems.CameraSystem;

using UnityEngine;
using UnityEngine.AI;

using Zenject;

public class CharacterRTSInstaller : MonoInstaller
{
	[SerializeField] private CharacterController characterController;
	[SerializeField] private NavMeshAgent navMeshAgent;
	[Space]
	[SerializeField] private NavigationController navigation;
	[SerializeField] private CharacterControllerRTS controller;
	[SerializeField] private Outlinable outline;
	[Space]
	[SerializeField] private CameraPivot cameraPivot;

	public override void InstallBindings()
	{
		Container.BindInstance(characterController);
		Container.BindInstance(navMeshAgent);

		Container.BindInstance(navigation);
		Container.Bind<IController>().FromMethod(() => controller);
		Container.BindInstance(outline);

		Container.BindInstance(cameraPivot);
	}
}