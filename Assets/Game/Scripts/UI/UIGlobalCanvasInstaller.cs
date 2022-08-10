using CodeStage.AdvancedFPSCounter;

using UnityEngine;

using Zenject;

namespace Game.UI.GlobalCanvas
{
    [CreateAssetMenu(fileName = "UIGlobalCanvasInstaller", menuName = "Installers/UIGlobalCanvasInstaller")]
    public class UIGlobalCanvasInstaller : ScriptableObjectInstaller<UIGlobalCanvasInstaller>
    {
        public GameObject GlobalCanvasPrefab;

        public override void InstallBindings()
        {
            Container.Bind<Canvas>()
                .FromResolveGetter<UIGlobalCanvas>(x => x.GetComponent<Canvas>())
                .AsSingle()
                .WhenInjectedInto<UIGlobalCanvas>();
            Container.Bind<UIGlobalCanvas>()
                .FromComponentInNewPrefab(GlobalCanvasPrefab)
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<AFPSCounter>()
                .FromMethod(ctx => ctx.Container.Resolve<UIGlobalCanvas>().GetComponentInChildren<AFPSCounter>(true))
                .AsSingle()
                .NonLazy();
        }
    }
}