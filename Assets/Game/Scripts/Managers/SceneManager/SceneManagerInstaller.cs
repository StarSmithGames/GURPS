using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Managers.SceneManager
{
	[CreateAssetMenu(fileName = "SceneManagerInstaller", menuName = "Installers/SceneManagerInstaller")]
	public class SceneManagerInstaller : ScriptableObjectInstaller<SceneManagerInstaller>
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<SignalSceneChanged>();

			Container.BindInterfacesAndSelfTo<SceneManager>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<SceneHandler>().AsSingle().NonLazy();
		}
	}

	public enum Scenes
	{
		Menu	= 0,
		Map		= 1,
		Polygon = 2,
	}
}