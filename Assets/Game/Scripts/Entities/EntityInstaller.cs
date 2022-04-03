using EPOOutline;

using UnityEngine;
using UnityEngine.AI;

using Zenject;

namespace Game.Entities
{
	public class EntityInstaller : MonoInstaller
	{
		[SerializeField] private Animator animator;
		[SerializeField] private NavMeshAgent navMeshAgent;
		[SerializeField] private CharacterController characterController;
		[Space]
		[SerializeField] private Transform model;
		[SerializeField] private Transform cameraPivot;
		[SerializeField] private CharacterController3D controller;
		[SerializeField] private NavigationController navigationController;
		[SerializeField] private Markers markers;
		[SerializeField] private Outlinable outline;
		[SerializeField] private Entity entity;

		public override void InstallBindings()
		{
			Container.BindInstance(animator);
			Container.BindInstance(navMeshAgent);
			Container.BindInstance(characterController);
			Container.BindInstance(model).WithId("Model");
			Container.BindInstance(cameraPivot).WithId("CameraPivot");
			Container.BindInstance(controller);
			Container.BindInstance(navigationController);
			Container.BindInstance(markers);
			Container.BindInstance(outline);
			Container.BindInstance(entity);
		}
	}
}