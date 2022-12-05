using Game.Entities;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem.Effects;
using Game.Systems.SheetSystem.Skills;

using Zenject;

namespace Game.Systems.SheetSystem
{
	public class SheetSystemInstaller : Installer<SheetSystemInstaller>
	{
		public override void InstallBindings()
		{
			EffectSystemInstaller.Install(Container);

			Container.BindFactory<CharacterData, CharacterSheet, CharacterSheet.Factory>().NonLazy();
			Container.BindFactory<ContainerData, ContainerSheet, ContainerSheet.Factory>().NonLazy();

			Container
				.BindFactory<ModelData, ISheet, SheetFactory>()
				.FromFactory<CustomSheetFactory>()
				.NonLazy();

			//SKILLS
			Container.BindFactory<PassiveSkillData, ICharacter, PassiveSkill, PassiveSkill.Factory>().AsSingle().NonLazy();
			Container
				.BindFactory<SkillData, ICharacter, ISkill, SkillFactory>()
				.FromFactory<CustomSkillFactory>()
				.NonLazy();
		}
	}
}