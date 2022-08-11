using EPOOutline;

using UnityEngine;

using Zenject;

public class ModelInstaller : MonoInstaller
{
	[SerializeField] protected Outlinable outline;

	public override void InstallBindings()
	{
		Container.BindInstance(outline);
	}
}