using Game.UI.CanvasSystem;
using UnityEngine;
using Zenject;

namespace Game.Systems.VFX
{
	[CreateAssetMenu(fileName = "VFXInstaller", menuName = "Installers/VFXInstaller")]
	public class VFXInstaller : ScriptableObjectInstaller<VFXInstaller>
	{
		public Pointer3D pointer3DPrefab;
		public Pointer2D pointer2DPrefab;

		public override void InstallBindings()
		{
			Container.BindFactory<Pointer3D, Pointer3D.Factory>()
					.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(0)
					.FromComponentInNewPrefab(pointer3DPrefab));

			Container.BindFactory<Pointer2D, Pointer2D.Factory>()
					.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
					.FromComponentInNewPrefab(pointer2DPrefab)
					.UnderTransform((x) => x.Container.Resolve<UISubCanvas>().VFXIndicators));
		}
	}
}