using Game.Managers.CharacterManager;
using Game.Managers.GameManager;
using Game.Systems.InteractionSystem;
using Game.Systems.InventorySystem;

using UnityEngine;

using Zenject;

public class ProjectInstaller : MonoInstaller
{
	[SerializeField] private GlobalSettings globalSettings;

	public override void InstallBindings()
	{
		SignalBusInstaller.Install(Container);

		Container.BindInstance(Container.InstantiateComponentOnNewGameObject<AsyncManager>());

		GameManagerInstaller.Install(Container);

		Container.BindInstance(globalSettings);

		CharacterManagerInstaller.Install(Container);

		InteractionInstaller.Install(Container);
	}
}