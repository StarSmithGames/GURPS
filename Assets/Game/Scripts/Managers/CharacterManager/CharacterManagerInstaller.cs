using Zenject;

namespace Game.Managers.CharacterManager
{
	public class CharacterManagerInstaller : Installer<CharacterManagerInstaller>
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<SignalCharacterChanged>();

			Container.BindInterfacesAndSelfTo<CharacterManager>().AsSingle();
		}
	}
}