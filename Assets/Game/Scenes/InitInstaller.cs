using Game.Managers.SceneManager;

using Zenject;

public class InitInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		Container.Resolve<SceneManager>().SwitchScene(Scenes.Menu);
	}
}