using Game.UI.CanvasSystem;
using UnityEngine;
using Zenject;

namespace Game.Systems.TooltipSystem
{
	[CreateAssetMenu(fileName = "TooltipSystemInstaller", menuName = "Installers/TooltipSystemInstaller")]
	public class TooltipSystemInstaller : ScriptableObjectInstaller<TooltipSystemInstaller>
	{
		public UIBattleTooltip battleTooltipPrefab;
		public UIObjectTooltip objectTooltipPrefab;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<TooltipSystem>().AsSingle();

			Container.Bind<UIBattleTooltip>()
				.FromComponentInNewPrefab(battleTooltipPrefab)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().transform)
				.WhenInjectedInto<TooltipSystem>();

			Container.Bind<UIObjectTooltip>()
				.FromComponentInNewPrefab(objectTooltipPrefab)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().transform)
				.AsSingle().NonLazy();
		}
	}
}