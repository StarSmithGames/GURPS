using Game.UI.CanvasSystem;

using Zenject;

namespace Game
{
	public class MenuInstaller : MonoInstaller<MenuInstaller>
	{
		public UISubCanvas subCanvas;

		public override void InstallBindings()
		{
			Container.BindInstance(subCanvas);
		}
	}
}