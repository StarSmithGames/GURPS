using Game.Managers.SceneManager;

using Zenject;

namespace Game
{
	public class InitInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.Resolve<SceneManager>().SwitchScene(SceneStorage.GetSceneName(Scenes.Menu));
		}
	}
}