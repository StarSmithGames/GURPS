using Game.Managers.GameManager;
using Game.Managers.SceneManager;
using Game.Managers.StorageManager;
using Game.Systems.CommandCenter;

using UnityEngine;
using Zenject;

namespace Game
{
	public class ProjectInstaller : MonoInstaller
	{
		public GlobalSettings globalSettings;

		[Space]
		public PlayerPrefsSaveLoad.Settings playerPrefsSettings;
		public DefaultData defaultData;

		public override void InstallBindings()
		{
			Debug.LogError("ProjectContext");
			SignalBusInstaller.Install(Container);

			Container.BindInstance(Container.InstantiateComponentOnNewGameObject<AsyncManager>());
			Container.BindInstance(Container.InstantiateComponentOnNewGameObject<CommandCenter>());

			BindSaveLoad();

			GameManagerInstaller.Install(Container);
			SceneManagerInstaller.Install(Container);

			Container.BindInstance(globalSettings);
		}

		private void BindSaveLoad()
		{
			Container.DeclareSignal<SignalSaveStorage>();
			Container.DeclareSignal<SignalStorageSaved>();
			Container.DeclareSignal<SignalStorageCleared>();
			Container.DeclareSignal<SignalStorageLoaded>();

			Container.BindInstance(playerPrefsSettings).WhenInjectedInto<PlayerPrefsSaveLoad>();
			Container.BindInstance(defaultData).WhenInjectedInto<PlayerPrefsSaveLoad>();

			Container.BindInterfacesTo<PlayerPrefsSaveLoad>().AsSingle();
			Container.BindInterfacesAndSelfTo<SaveLoadOverseer>().AsSingle().NonLazy();
		}
	}
}