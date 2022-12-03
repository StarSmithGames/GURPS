using Game.Systems.CombatDamageSystem;
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
			SkillSystemInstaller.Install(Container);
		}
	}
}