using Game.Managers.SceneManager;
using Game.UI;
using Game.UI.CanvasSystem;

using UnityEngine;

using Zenject;

namespace Game.Managers.TransitionManager
{
	[CreateAssetMenu(fileName = "TransitionManagerInstaller", menuName = "Installers/TransitionManagerInstaller")]
	public class TransitionManagerInstaller : ScriptableObjectInstaller<TransitionManagerInstaller>
	{
		public UIFadeTransition fadePrefab;

		public override void InstallBindings()
		{
			Container
				.BindFactory<UIFadeTransition, UIFadeTransition.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
				.FromComponentInNewPrefab(fadePrefab)
				.UnderTransform((x) => x.Container.Resolve<UIGlobalCanvas>().Transitions))
				.WhenInjectedInto<TransitionManager>();

			Container.BindInterfacesAndSelfTo<TransitionManager>().AsSingle();
		}
	}

	public enum Transitions
	{
		Fade = 0,
	}
}