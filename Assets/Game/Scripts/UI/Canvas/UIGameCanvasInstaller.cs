using Game.UI.Windows;
using UnityEngine;
using Zenject;

namespace Game.UI.CanvasSystem
{
	[CreateAssetMenu(fileName = "UIGameCanvasInstaller", menuName = "Installers/UIGameCanvasInstaller")]
	public class UIGameCanvasInstaller : ScriptableObjectInstaller<UIGameCanvasInstaller>
	{
		[Header("Menu")]
		public WindowMainMenu mainMenuWindow;
		public WindowLoadingCommit loadingCommitWindow;
		public WindowPreferences preferencesWindow;

		private void BindMenu()
		{
			//Windows
			Container.Bind<WindowMainMenu>()
				.FromComponentInNewPrefab(mainMenuWindow)
				.UnderTransform(x => x.Container.Resolve<UIGameCanvas>().Windows)
				.AsSingle()
				.NonLazy();

			Container.Bind<WindowLoadingCommit>()
				.FromComponentInNewPrefab(loadingCommitWindow)
				.UnderTransform(x => x.Container.Resolve<UIGameCanvas>().Windows)
				.AsSingle()
				.NonLazy();

			Container.Bind<WindowPreferences>()
				.FromComponentInNewPrefab(preferencesWindow)
				.UnderTransform(x => x.Container.Resolve<UIGameCanvas>().Windows)
				.AsSingle()
				.NonLazy();
		}
	}
}