using EPOOutline;

using Game.Entities.Models;
using Game.Systems.InteractionSystem;

using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

public class ModelInstaller : MonoInstaller
{
	[Title("Model")]
	public Model model;
	public Outlinable outline;
	public InteractionPoint interactionPoint;

	public override void InstallBindings()
	{
		Container.BindInstance(outline);
		Container.BindInstance(interactionPoint);
		BindModel();
	}

	protected virtual void BindModel()
	{
		Container.BindInstance<Model>(model);
	}
}