using Game.UI.Windows;

using UnityEngine;

using Zenject;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "UISubCanvasMapInstaller", menuName = "Installers/UISubCanvasMapInstaller")]
    public class UISubCanvasMapInstaller : ScriptableObjectInstaller<UISubCanvasMapInstaller>
    {
		public GameObject SubCanvasPrefab;
		[Header("Menu")]
		public WindowMainMenu mainMenuWindow;
		public WindowLoadingCommit loadingCommitWindow;
		public WindowPreferences preferencesWindow;

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
			BindMenu();
		}

		private void BindMenu()
		{
			//Windows
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
	}
}