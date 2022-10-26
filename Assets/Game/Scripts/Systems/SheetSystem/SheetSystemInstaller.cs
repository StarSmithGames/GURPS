using Zenject;

namespace Game.Systems.SheetSystem
{
	public class SheetSystemInstaller : Installer<SheetSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindFactory<InstantEffectData, ISheet, InstantEffect, InstantEffect.Factory>().NonLazy();
			Container.BindFactory<ProcessEffectData, ISheet, ProcessEffect, ProcessEffect.Factory>().NonLazy();
			Container.BindFactory<InflictEffectData, ISheet, InflictEffect, InflictEffect.Factory>().NonLazy();

			Container
				.BindFactory<EffectData, ISheet, IEffect, EffectFactory>()
				.FromFactory<CustomEffectFactory>()
				.NonLazy();
		}
	}
}