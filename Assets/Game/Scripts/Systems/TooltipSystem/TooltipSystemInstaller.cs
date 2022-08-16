using Game.UI;

using UnityEngine;

using Zenject;

namespace Game.Systems.TooltipSystem
{
	[CreateAssetMenu(fileName = "TooltipSystemInstaller", menuName = "Installers/TooltipSystemInstaller")]
	public class TooltipSystemInstaller : ScriptableObjectInstaller<TooltipSystemInstaller>
	{
		public TooltipSystem tooltip;

		public override void InstallBindings()
		{
			Container.Bind<TooltipSystem>()
				.FromComponentInNewPrefab(tooltip)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().transform)
				.AsSingle()
				.NonLazy();
		}
	}
}