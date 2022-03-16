using UnityEngine;
using UnityEngine.AI;

using Zenject;

public class CharacterInstaller : MonoInstaller
{
	[SerializeField] private Animator animator;
	[SerializeField] private CapsuleCollider collider;
	[SerializeField] private Rigidbody rigidbody;
	[SerializeField] private NavMeshAgent navMeshAgent;
	[Space]
	[SerializeField] private Transform root;
	[SerializeField] private Transform model;
	[SerializeField] private PointClickController controller;
	[SerializeField] private Mover mover;
	[SerializeField] private SensorHandler sensor;

	public override void InstallBindings()
	{
		Container.BindInstance(animator);
		Container.BindInstance(collider);
		Container.BindInstance(rigidbody);
		Container.BindInstance(navMeshAgent);
		Container.BindInstance(root).WithId("Root");
		Container.BindInstance(model).WithId("Model");
		Container.BindInstance(controller);
		Container.BindInstance(mover);
		Container.BindInstance(sensor);
	}
}