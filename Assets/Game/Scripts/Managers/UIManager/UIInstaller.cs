using Game.Systems.BattleSystem;
using Game.Systems.ContextMenu;
using Game.Systems.InventorySystem;

using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "UIInstaller", menuName = "Installers/UIInstaller")]
public class UIInstaller : ScriptableObjectInstaller<UIInstaller>
{
	[Header("General")]
	[SerializeField] private UIAvatar avatarPrefab;
	[SerializeField] private UIContextAction contextActionPrefab;
	[Header("Inventory-Container")]
	[SerializeField] private UIItemCursor itemCursorPrefab;
	[SerializeField] private UIContainerWindow chestPopupWindowPrefab;
	[Header("Battle")]
	[SerializeField] private UITurn turnPrefab;
	[SerializeField] private GameObject turnSeparatePrefab;
	[Space]
	[SerializeField] private UIAction actionPrefab;

	public override void InstallBindings()
	{
		Container.BindInstance(GameObject.FindObjectOfType<UIManager>());//stub
		Container.Bind<UIWindowsManager>().WhenInjectedInto<UIManager>();

		Container.BindFactory<UIAvatar, UIAvatar.Factory>()
			.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(2)
			.FromComponentInNewPrefab(avatarPrefab));

		BindContextMenu();

		BindInventoryContainer();

		BindBattleSystem();

		BindActionFactory();
	}

	private void BindContextMenu()
	{
		Container.BindFactory<UIContextAction, UIContextAction.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(3)
				.FromComponentInNewPrefab(contextActionPrefab))
				.WhenInjectedInto<ContextMenuHandler>();
	}

	private void BindInventoryContainer()
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

	private void BindActionFactory()
	{
		Container.BindFactory<UIAction, UIAction.Factory>()
			.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(7)
			.FromComponentInNewPrefab(actionPrefab));
	}
}