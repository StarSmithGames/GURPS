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
		public Transform modelRoot;
		[Space]
		public Animator animator;
		public NavMeshAgent navMeshAgent;
		public CharacterController characterController;
		[Space]
		public AnimatorController animatorControl;
		public CharacterController3D controller;
		public NavigationController navigationController;

		public override void InstallBindings()
		{
			base.InstallBindings();
			Container.BindInstance(animator);
			Container.BindInstance(navMeshAgent);
			Container.BindInstance(characterController);
			Container.BindInstance(modelRoot).WithId("Model");
			Container.BindInstance(animatorControl);
			Container.BindInstance<IController>(controller);
			Container.BindInstance(navigationController);
		}

		protected override void BindModel()
		{
			Container.BindInstance<IEntityModel>(base.model as IEntityModel);
		}
	}
}