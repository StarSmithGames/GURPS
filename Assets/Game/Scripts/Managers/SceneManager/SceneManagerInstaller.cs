using UnityEngine;

using Zenject;

namespace Game.Managers.SceneManager
{
	[CreateAssetMenu(fileName = "SceneManagerInstaller", menuName = "Installers/SceneManagerInstaller")]
	public class SceneManagerInstaller : Installer<SceneManagerInstaller>
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<SignalSceneChanged>();

			Container.BindInterfacesAndSelfTo<SceneManager>().AsSingle().NonLazy();
		}
	}
}