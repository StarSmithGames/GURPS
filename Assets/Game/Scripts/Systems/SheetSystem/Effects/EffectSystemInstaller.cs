using Zenject;

namespace Game.Systems.SheetSystem.Effects
{
	public class EffectSystemInstaller : Installer<EffectSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindFactory<InstantEffectData, ISheet, InstantEffect, InstantEffect.Factory>().NonLazy();
			Container.BindFactory<PersistentEffectData, ISheet, ProcessEffect, ProcessEffect.Factory>().NonLazy();
			Container.BindFactory<InflictEffectData, ISheet, InflictEffect, InflictEffect.Factory>().NonLazy();

			Container
				.BindFactory<EffectData, ISheet, IEffect, EffectFactory>()
				.FromFactory<CustomEffectFactory>()
				.NonLazy();
		}
	}
}