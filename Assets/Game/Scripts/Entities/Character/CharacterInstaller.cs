using UnityEngine;
using UnityEngine.AI;

using Zenject;

namespace Game.Entities
{
	public class CharacterInstaller : MonoInstaller
	{
		[SerializeField] private Animator animator;
		[SerializeField] private NavMeshAgent navMeshAgent;
		[SerializeField] private CharacterController characterController;
		[Space]
		[SerializeField] private Transform model;
		[SerializeField] private Transform cameraPivot;
		[SerializeField] private CharacterController3D controller;
		[SerializeField] private MarkerController markerController;

		public override void InstallBindings()
		{
			Container.BindInstance(animator);
			Container.BindInstance(navMeshAgent);
			Container.BindInstance(characterController);
			Container.BindInstance(model).WithId("Model");
			Container.BindInstance(cameraPivot).WithId("CameraPivot");
			Container.BindInstance(controller);
			Container.BindInstance(markerController);
		}
	}
}