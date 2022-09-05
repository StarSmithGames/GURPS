using Game.Entities.Models;

using Zenject;

namespace Game.Systems.CombatDamageSystem
{
	public class CombatDamageSystemInstaller : Installer<CombatDamageSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<CombatDamageSystem>().AsSingle().NonLazy();

			//Bind Character Attack Factories
			Container.BindFactory<ICharacterModel, ICharacterModel, TaskCharacterAttackCharacter, TaskCharacterAttackCharacter.Factory>().AsSingle();
			Container.BindFactory<ICharacterModel, IDamageable, TaskCharacterAttackDamageable, TaskCharacterAttackDamageable.Factory>().AsSingle();

			Container
				.BindFactory<ICharacterModel, IDamageable, ITaskAction, CharacterAttackFactory>()
				.FromFactory<CustomCharacterAttackFactory>()
				.NonLazy();
		}
	}
}