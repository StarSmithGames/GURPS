using UnityEngine;

using Zenject;

namespace Game.Systems.CursorSystem
{
	[CreateAssetMenu(fileName = "CursorSystemInstaller", menuName = "Installers/CursorSystemInstaller")]
	public class CursorSystemInstaller : ScriptableObjectInstaller<CursorSystemInstaller>
	{
		public CursorSettings settings;

		public override void InstallBindings()
		{
			Container.BindInstance(settings).WhenInjectedInto<CursorSystem>();
			Container.BindInterfacesAndSelfTo<CursorSystem>().AsSingle().NonLazy();
		}
	}
}