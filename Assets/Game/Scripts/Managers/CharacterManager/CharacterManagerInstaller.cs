using Game.Entities;
using Zenject;

namespace Game.Managers.CharacterManager
{
	public class CharacterManagerInstaller : Installer<CharacterManagerInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindFactory<CharacterData, Character, Character.Factory>().WhenInjectedInto<CharacterManager>();

			Container.BindInterfacesAndSelfTo<CharacterManager>().AsSingle();
		}
	}
}