using Game.Managers.CharacterManager;
using Game.Managers.PartyManager;
using Game.Systems.BattleSystem;
using Game.Systems.ContextMenu;
using Game.Systems.InventorySystem;
using Game.UI.Windows;

using UnityEngine;

using Zenject;

namespace Game.UI
{
	[CreateAssetMenu(fileName = "UISubCanvasLocationInstaller", menuName = "Installers/UISubCanvasLocationInstaller")]
	public class UISubCanvasLocationInstaller : ScriptableObjectInstaller<UISubCanvasLocationInstaller>
	{
		[Header("Menu")]
		public WindowMainMenu mainMenuWindow;
		public WindowLoadingCommit loadingCommitWindow;
		public WindowPreferences preferencesWindow;
		[Header("Character")]
		public UIAvatar avatarPrefab;
		[Header("Inventory")]
		public UIItemCursor itemCursorPrefab;
		public UIContainerWindow chestPopupWindowPrefab;
		[Header("Battle")]
		public UITurn turnPrefab;
		public GameObject turnSeparatePrefab;
		[Space]
		public UIAction actionPrefab;
		

		public override void InstallBindings()
		{
			Container.Bind<Canvas>()
				.FromResolveGetter<UISubCanvas>(x => x.GetComponent<Canvas>())
				.AsSingle()
				.WhenInjectedInto<UIGlobalCanvas>();

			//Windows
			BindMenu();

			BindCharacterWindows();

			BindInventory();

			BindBattleSystem();

			BindActions();
		}

		private void BindMenu()
		{
			Container.Bind<WindowMainMenu>()
				.FromComponentInNewPrefab(mainMenuWindow)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().Windows)
				.AsSingle()
				.NonLazy();

			Container.Bind<WindowLoadingCommit>()
				.FromComponentInNewPrefab(loadingCommitWindow)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().Windows)
				.AsSingle()
				.NonLazy();

			Container.Bind<WindowPreferences>()
				.FromComponentInNewPrefab(preferencesWindow)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().Windows)
				.AsSingle()
				.NonLazy();
		}

		private void BindCharacterWindows()
		{
			Container.BindFactory<UIAvatar, UIAvatar.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(2)
				.FromComponentInNewPrefab(avatarPrefab));
		}

		private void BindInventory()
		{
			Container.BindFactory<UIContainerWindow, UIContainerWindow.Factory>()
			   .FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
			   .FromComponentInNewPrefab(chestPopupWindowPrefab));

			Container.BindInstance(Container.InstantiatePrefabForComponent<UIItemCursor>(itemCursorPrefab));

			Container.BindInterfacesAndSelfTo<InventoryContainerHandler>().AsSingle();
		}

		private void BindBattleSystem()
		{
			Container.BindFactory<UITurn, UITurn.Factory>()
					.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(6)
					.FromComponentInNewPrefab(turnPrefab));

			Container.BindInstance(Container.InstantiatePrefab(turnSeparatePrefab)).WithId("TurnSeparate");
		}

		private void BindActions()
		{
			Container.BindFactory<UIAction, UIAction.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(7)
				.FromComponentInNewPrefab(actionPrefab));
		}
	}
}