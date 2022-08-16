using CodeStage.AdvancedFPSCounter;

using Game.Managers.TransitionManager;
using Game.Systems.InventorySystem;
using Game.UI.Windows;

using UnityEngine;

using Zenject;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "UIGlobalCanvasInstaller", menuName = "Installers/UIGlobalCanvasInstaller")]
    public class UIGlobalCanvasInstaller : ScriptableObjectInstaller<UIGlobalCanvasInstaller>
    {
        public GameObject GlobalCanvasPrefab;
        [Space]
        public WindowInputGenericDialogue inputGenericDialogue;
        [Space]
        public WindowInfinityLoading infinityLoadingWindow;
        public UICommit commitPrefab;

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
            Container.Bind<WindowInfinityLoading>()
                .FromComponentInNewPrefab(infinityLoadingWindow)
                .UnderTransform(x => x.Container.Resolve<UIGlobalCanvas>().transform.Find("Windows"))
                .AsSingle()
                .NonLazy();

            Container.Bind<WindowInputGenericDialogue>()
                .FromComponentInNewPrefab(inputGenericDialogue)
                .UnderTransform(x => x.Container.Resolve<UIGlobalCanvas>().transform.Find("Windows"))
                .AsSingle()
                .NonLazy();

            //Factories
            Container
                 .BindFactory<UICommit, UICommit.Factory>()
                 .FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
                 .FromComponentInNewPrefab(commitPrefab));
        }
    }
}