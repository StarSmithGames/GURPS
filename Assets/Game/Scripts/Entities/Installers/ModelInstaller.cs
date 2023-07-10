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
	public MarkPoint markPoint;

	public override void InstallBindings()
	{
		Container.BindInstance(outline);
		Container.BindInstance(interactionPoint);
		if (markPoint != null)
		{
			Container.BindInstance(markPoint);
		}
		BindModel();
	}

	protected virtual void BindModel()
	{
		Container.BindInstance<Model>(model);
	}
}