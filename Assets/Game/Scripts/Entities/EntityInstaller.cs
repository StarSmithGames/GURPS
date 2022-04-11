using EPOOutline;

using Game.Systems.CameraSystem;

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
		[SerializeField] private CameraPivot cameraPivot;
		[SerializeField] private AnimatorControl animatorControl;
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
			Container.BindInstance(cameraPivot);
			Container.BindInstance(animatorControl);
			Container.BindInstance(controller);
			Container.BindInstance(navigationController);
			Container.BindInstance(markers);
			Container.BindInstance(outline);
			Container.BindInstance<IEntity>(entity);
		}
	}
}