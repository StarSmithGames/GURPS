using Game.UI;
using Game.UI.CanvasSystem;

using UnityEngine;

using Zenject;

namespace Game.Systems.ContextMenu
{
	[CreateAssetMenu(fileName = "ContextMenuSystemInstaller", menuName = "Installers/ContextMenuSystemInstaller")]
	public class ContextMenuSystemInstaller : ScriptableObjectInstaller<ContextMenuSystemInstaller>
	{
		public WindowContextMenu contextMenuWindow;
		public UIContextAction contextActionPrefab;

		public override void InstallBindings()
		{
			Container.BindFactory<UIContextAction, UIContextAction.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(3)
				.FromComponentInNewPrefab(contextActionPrefab))
				.WhenInjectedInto<WindowContextMenu>();

			Container.Bind<WindowContextMenu>()
				.FromComponentInNewPrefab(contextMenuWindow)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().transform)
				.AsSingle()
				.NonLazy();

			Container.BindInterfacesAndSelfTo<ContextMenuSystem>().AsSingle().NonLazy();
		}
	}
}