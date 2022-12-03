using Game.Entities;
using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public class SkillSystemInstaller : Installer<SkillSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindFactory<PassiveSkillData, ICharacter, PassiveSkill, PassiveSkill.Factory>().NonLazy();

			Container
				.BindFactory<SkillData, ICharacter, ISkill, SkillFactory>()
				.FromFactory<CustomSkillFactory>()
				.NonLazy();
		}
	}
}