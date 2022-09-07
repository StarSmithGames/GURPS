using Game.Entities.Models;

using Zenject;

namespace Game.Systems.CombatDamageSystem
{
	public class CombatDamageSystemInstaller : Installer<CombatDamageSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<CombatDamageSystem>().AsSingle().NonLazy();

			Container.BindFactory<ICharacterModel, IDamageable, CombatBase, CombatBase.Factory>().NonLazy();
			Container.BindFactory<ICharacterModel, IDamageable, CombatHumanoid, CombatHumanoid.Factory>().NonLazy();

			Container
				.BindFactory<ICharacterModel, IDamageable, CombatBase, CombatFactory>()
				.FromFactory<CustomCombatFactory>()
				.NonLazy();
		}
	}
}