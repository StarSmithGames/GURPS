using Game.Entities.Models;
using Game.Systems.AnimatorController;
using Game.Systems.NavigationSystem;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.AI;

namespace Game.Entities
{
	public class EntityInstaller : ModelInstaller
	{
		[Title("Entity")]
		[SerializeField] private EntityModel entity;
		[SerializeField] private Transform model;
		[Space]
		[SerializeField] private Animator animator;
		[SerializeField] private NavMeshAgent navMeshAgent;
		[SerializeField] private CharacterController characterController;
		[Space]
		[SerializeField] private AnimatorController animatorControl;
		[SerializeField] private CharacterController3D controller;
		[SerializeField] private NavigationController navigationController;

		public override void InstallBindings()
		{
			base.InstallBindings();
			Container.BindInstance(animator);
			Container.BindInstance(navMeshAgent);
			Container.BindInstance(characterController);
			Container.BindInstance(model).WithId("Model");
			Container.BindInstance(animatorControl);
			Container.BindInstance<IController>(controller);
			Container.BindInstance(navigationController);
			Container.BindInstance<IEntityModel>(entity);
		}
	}
}