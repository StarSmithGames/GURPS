using UnityEngine;
using UnityEngine.AI;

using Zenject;

public class CharacterInstaller : MonoInstaller
{
	[SerializeField] private Animator animator;
	[SerializeField] private NavMeshAgent navMeshAgent;
	[SerializeField] private CharacterController characterController;
	[Space]
	[SerializeField] private Transform model;
	[SerializeField] private PointClickInput input;
	[SerializeField] private CharacterThirdPersonController controller;

	public override void InstallBindings()
	{
		Container.BindInstance(animator);
		Container.BindInstance(navMeshAgent);
		Container.BindInstance(characterController);
		Container.BindInstance(model).WithId("Model");
		Container.BindInstance(input);
		Container.BindInstance(controller);
	}
}