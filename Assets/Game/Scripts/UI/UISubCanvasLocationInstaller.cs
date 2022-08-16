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
		[Space]
		public UIAvatar avatarPrefab;
		[Header("Inventory")]
		public UIItemCursor itemCursorPrefab;
		public UIContainerWindow chestPopupWindowPrefab;
		[Header("Battle")]
		public UITurn turnPrefab;
		public GameObject turnSeparatePrefab;
		[Space]
		public UIAction actionPrefab;
		public UIContextAction contextActionPrefab;

		public override void InstallBindings()
		{
			Container.Bind<Canvas>()
				.FromResolveGetter<UISubCanvas>(x => x.GetComponent<Canvas>())
				.AsSingle()
				.WhenInjectedInto<UIGlobalCanvas>();

			//Windows
			BindMenu();

			//Factories
			Container.BindFactory<UIAvatar, UIAvatar.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(2)
				.FromComponentInNewPrefab(avatarPrefab));


			BindContexMenu();

			BindInventory();

			BindBattleSystem();

			BindActions();
		}

		private void BindMenu()
		{
			Container.Bind<WindowMainMenu>()
				.FromComponentInNewPrefab(mainMenuWindow)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().transform.Find("Windows"))
				.AsSingle()
				.NonLazy();

			Container.Bind<WindowLoadingCommit>()
				.FromComponentInNewPrefab(loadingCommitWindow)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().transform.Find("Windows"))
				.AsSingle()
				.NonLazy();

			Container.Bind<WindowPreferences>()
				.FromComponentInNewPrefab(preferencesWindow)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().transform.Find("Windows"))
				.AsSingle()
				.NonLazy();
		}

		private void BindInventory()
		{
			Container.BindFactory<UIContainerWindow, UIContainerWindow.Factory>()
			   .FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
			   .FromComponentInNewPrefab(chestPopupWindowPrefab));

			Container.BindInstance(Container.InstantiatePrefabForComponent<UIItemCursor>(itemCursorPrefab));

			Container.BindInterfacesAndSelfTo<InventoryContainerHandler>().AsSingle();
		}

		private void BindContexMenu()
		{
			Container.BindFactory<UIContextAction, UIContextAction.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(3)
				.FromComponentInNewPrefab(contextActionPrefab))
				.WhenInjectedInto<ContextMenuHandler>();

			Container.BindInterfacesAndSelfTo<ContextMenuHandler>().AsSingle();
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