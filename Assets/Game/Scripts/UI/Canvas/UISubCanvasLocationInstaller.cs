using Game.Managers.PartyManager;
using Game.Systems.BattleSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using UnityEngine;

using Zenject;

namespace Game.UI.CanvasSystem
{
	[CreateAssetMenu(fileName = "UISubCanvasLocationInstaller", menuName = "Installers/UISubCanvasLocationInstaller")]
	public class UISubCanvasLocationInstaller : ScriptableObjectInstaller<UISubCanvasLocationInstaller>
	{

		[Header("Character")]
		public UIAvatar avatarPrefab;
		[Header("Inventory")]
		public UIItemCursor itemCursorPrefab;
		public UIContainerWindow chestPopupWindowPrefab;
		[Header("Battle")]
		public UITurn turnPrefab;
		public GameObject turnSeparatePrefab;
		[Header("Sheet")]
		public UIActionPoint actionPointPrefab;
		public UIAction actionPrefab;


		public override void InstallBindings()
		{
			//Windows
			BindCharacterWindows();

			BindInventory();

			BindBattleSystem();

			BindActionPoints();
			BindActions();
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

		private void BindActionPoints()
		{
			Container.BindFactory<UIActionPoint, UIActionPoint.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(7)
				.FromComponentInNewPrefab(actionPointPrefab));
		}

		private void BindActions()
		{
			Container.BindFactory<UIAction, UIAction.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(15)
				.FromComponentInNewPrefab(actionPrefab));
		}
	}
}