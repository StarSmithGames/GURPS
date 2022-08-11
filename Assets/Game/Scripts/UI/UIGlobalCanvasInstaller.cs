using CodeStage.AdvancedFPSCounter;

using Game.UI.Windows;

using UnityEngine;

using Zenject;

namespace Game.UI.GlobalCanvas
{
    [CreateAssetMenu(fileName = "UIGlobalCanvasInstaller", menuName = "Installers/UIGlobalCanvasInstaller")]
    public class UIGlobalCanvasInstaller : ScriptableObjectInstaller<UIGlobalCanvasInstaller>
    {
        public GameObject GlobalCanvasPrefab;
        [Space]
        public WindowInfinityLoading infinityLoadingWindow;

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

            //Windows
            Container.Bind<WindowsManager>().WhenInjectedInto<UIGlobalCanvas>();//global window manager

            Container.Bind<WindowInfinityLoading>()
                .FromComponentInNewPrefab(infinityLoadingWindow)
                .UnderTransform(x => x.Container.Resolve<UIGlobalCanvas>().transform.Find("Windows"))
                .AsSingle()
                .NonLazy();
        }
    }
}