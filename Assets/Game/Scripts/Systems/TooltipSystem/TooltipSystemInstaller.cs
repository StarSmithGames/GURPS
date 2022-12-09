using Game.UI.CanvasSystem;
using UnityEngine;
using Zenject;

namespace Game.Systems.TooltipSystem
{
	[CreateAssetMenu(fileName = "TooltipSystemInstaller", menuName = "Installers/TooltipSystemInstaller")]
	public class TooltipSystemInstaller : ScriptableObjectInstaller<TooltipSystemInstaller>
	{
		public UIBattleTooltip battleTooltip;
		public UITooltip uiTooltip;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<TooltipSystem>().AsSingle();

			Container.Bind<UIBattleTooltip>()
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