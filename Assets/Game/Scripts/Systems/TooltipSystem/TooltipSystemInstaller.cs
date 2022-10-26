using Game.UI.CanvasSystem;
using UnityEngine;
using Zenject;

namespace Game.Systems.TooltipSystem
{
	[CreateAssetMenu(fileName = "TooltipSystemInstaller", menuName = "Installers/TooltipSystemInstaller")]
	public class TooltipSystemInstaller : ScriptableObjectInstaller<TooltipSystemInstaller>
	{
		public BattleTooltip battleTooltip;
		public UITooltip uiTooltip;

		public override void InstallBindings()
		{
			Container.Bind<BattleTooltip>()
				.FromComponentInNewPrefab(battleTooltip)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().transform)
				.AsSingle()
				.NonLazy();

			Container.Bind<UITooltip>()
				.FromComponentInNewPrefab(uiTooltip)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().transform)
				.AsSingle()
				.NonLazy();
		}
	}
}