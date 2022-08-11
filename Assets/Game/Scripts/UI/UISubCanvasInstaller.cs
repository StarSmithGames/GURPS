using Game.UI.Windows;

using UnityEngine;

using Zenject;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "UISubCanvasInstaller", menuName = "Installers/UISubCanvasInstaller")]
    public class UISubCanvasInstaller : ScriptableObjectInstaller<UISubCanvasInstaller>
    {
		public GameObject SubCanvasPrefab;
		[Space]
		public WindowMainMenu mainMenuWindow;

		public override void InstallBindings()
		{
			Container.Bind<Canvas>()
				.FromResolveGetter<UISubCanvas>(x => x.GetComponent<Canvas>())
				.AsSingle()
				.WhenInjectedInto<UIGlobalCanvas>();
			Container.Bind<UISubCanvas>()
				.FromComponentInNewPrefab(SubCanvasPrefab)
				.AsSingle()
				.NonLazy();

			//Windows
			Container.Bind<WindowsManager>().WhenInjectedInto<UISubCanvas>();//sub window manager

			Container.Bind<WindowMainMenu>()
				.FromComponentInNewPrefab(mainMenuWindow)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().transform.Find("Windows"))
				.AsSingle()
				.NonLazy();
		}
	}
}