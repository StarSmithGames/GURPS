using Game.Entities;
using Zenject;

namespace Game.Managers.CharacterManager
{
	public class CharacterManagerInstaller : Installer<CharacterManagerInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindFactory<CharacterData, PlayableCharacter, PlayableCharacter.Factory>().WhenInjectedInto<CharacterManager>();
			Container.BindFactory<CharacterData, NonPlayableCharacter, NonPlayableCharacter.Factory>().WhenInjectedInto<CharacterManager>();

			Container.BindInterfacesAndSelfTo<CharacterManager>().AsSingle();
		}
	}
}