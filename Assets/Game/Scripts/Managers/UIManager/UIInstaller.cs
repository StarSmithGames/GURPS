using Game.Systems.InventorySystem;

using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "UIInstaller", menuName = "Installers/UIInstaller")]
public class UIInstaller : ScriptableObjectInstaller<UIInstaller>
{
	[SerializeField] private UIItemCursor itemCursorPrefab;
	[SerializeField] private UIContainerWindow chestPopupWindowPrefab;

	public override void InstallBindings()
	{
		Container.BindInstance(GameObject.FindObjectOfType<UIManager>());//stub

		Container.Bind<UIWindowsManager>().WhenInjectedInto<UIManager>();

		Container.BindFactory<UIContainerWindow, UIContainerWindow.Factory>()
			.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
			.FromComponentInNewPrefab(chestPopupWindowPrefab));

		Container.BindInstance(Container.InstantiatePrefabForComponent<UIItemCursor>(itemCursorPrefab));

		Container.BindInterfacesAndSelfTo<InventoryContainerHandler>().AsSingle();
	}
}