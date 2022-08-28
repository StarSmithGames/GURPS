using Game.Managers.CharacterManager;
using Game.Managers.PartyManager;
using Game.UI.Windows;

using UnityEngine;

using Zenject;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "UISubCanvasMapInstaller", menuName = "Installers/UISubCanvasMapInstaller")]
    public class UISubCanvasMapInstaller : ScriptableObjectInstaller<UISubCanvasMapInstaller>
    {
		public WindowMainMenu mainMenuWindow;
		public WindowLoadingCommit loadingCommitWindow;
		public WindowPreferences preferencesWindow;

		public override void InstallBindings()
		{
			BindMenu();
		}

		private void BindMenu()
		{
			//Windows
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
	}
}