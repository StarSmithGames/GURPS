using Game.Managers.SceneManager;

using UnityEngine;

using Zenject;

namespace Game.Managers.TransitionManager
{
	[CreateAssetMenu(fileName = "SceneManagerInstaller", menuName = "Installers/SceneManagerInstaller")]
	public class TransitionManagerInstaller : ScriptableObjectInstaller<SceneManagerInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<TransitionManager>().NonLazy();
		}
	}

	public enum Transitions
	{
		Fade,
	}
}