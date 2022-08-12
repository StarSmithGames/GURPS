using Game.Managers.CharacterManager;
using Game.Managers.GameManager;
using Game.Managers.StorageManager;
using Game.Systems.InteractionSystem;
using Game.Systems.InventorySystem;

using UnityEngine;

using Zenject;

public class ProjectInstaller : MonoInstaller
{
	public GlobalSettings globalSettings;

	[Space]
	public PlayerPrefsSaveLoad.Settings playerPrefsSettings;
	public DefaultData defaultData;

	public override void InstallBindings()
	{
		SignalBusInstaller.Install(Container);

		Container.BindInstance(Container.InstantiateComponentOnNewGameObject<AsyncManager>());

		BindSaveLoad();

		GameManagerInstaller.Install(Container);

		Container.BindInstance(globalSettings);

		CharacterManagerInstaller.Install(Container);
	}

	private void BindSaveLoad()
	{
		Container.DeclareSignal<SignalSaveStorage>();
		Container.DeclareSignal<SignalStorageSaved>();
		Container.DeclareSignal<SignalStorageLoaded>();

		Container.BindInstance(playerPrefsSettings).WhenInjectedInto<PlayerPrefsSaveLoad>();
		Container.BindInstance(defaultData).WhenInjectedInto<PlayerPrefsSaveLoad>();

		Container.BindInterfacesTo<PlayerPrefsSaveLoad>().AsSingle();
		Container.BindInterfacesAndSelfTo<SaveLoadOverseer>().AsSingle().NonLazy();
	}
}