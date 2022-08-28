using EPOOutline;

using Game.Systems.InteractionSystem;

using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

public class ModelInstaller : MonoInstaller
{
	[Title("Model")]
	[SerializeField] private Outlinable outline;
	[SerializeField] private InteractionPoint interactionPoint;

	public override void InstallBindings()
	{
		Container.BindInstance(outline);
		Container.BindInstance(interactionPoint);
	}
}