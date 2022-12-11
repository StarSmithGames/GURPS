using Game.UI.CanvasSystem;
using Game.UI.Windows;

using UnityEngine;
using Zenject;

namespace Game.Systems.VFX
{
	[CreateAssetMenu(fileName = "VFXInstaller", menuName = "Installers/VFXInstaller")]
	public class VFXInstaller : ScriptableObjectInstaller<VFXInstaller>
	{
		public Pointer3D pointer3DPrefab;
		public Pointer2D pointer2DPrefab;
		[Header("Lines")]
		public PointerVFX pointerPrefab;
		public LineTargetVFX lineTargetPrefab;
		[Header("Areas")]
		public RadialAreaDecalVFX radialAreaDecalPrefab;
		[Header("Projectiles")]
		public ElectricBallProjectileVFX electricBallPrefab;
		public ElectricBallProjectileVFX electricBall2Prefab;

		public override void InstallBindings()
		{
			Container
				.BindFactory<Pointer3D, Pointer3D.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(0)
				.FromComponentInNewPrefab(pointer3DPrefab));

			Container
				.BindFactory<Pointer2D, Pointer2D.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
				.FromComponentInNewPrefab(pointer2DPrefab)
				.UnderTransform((x) => x.Container.Resolve<UISubCanvas>().VFXIndicators));

			Container
				.Bind<PointerVFX>()
				.FromComponentInNewPrefab(pointerPrefab)
				.AsSingle()
				.NonLazy();

			BindLines();
			BindAreas();
			BindProjectiles();
		}

		private void BindLines()
		{
			Container
				.BindFactory<LineTargetVFX, LineTargetVFX.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
				.FromComponentInNewPrefab(lineTargetPrefab));
		}

		private void BindAreas()
		{
			Container
				.BindFactory<RadialAreaDecalVFX, RadialAreaDecalVFX.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
				.FromComponentInNewPrefab(radialAreaDecalPrefab));
		}

		private void BindProjectiles()
		{
			Container
				.BindFactory<ElectricBallProjectileVFX, ElectricBallProjectileVFX.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
				.FromComponentInNewPrefab(electricBallPrefab));

			Container
				.BindFactory<ElectricBallProjectileVFX, ElectricBallProjectileVFX.Factory>()
				.WithId("Version2")
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
				.FromComponentInNewPrefab(electricBall2Prefab));
		}
	}
}