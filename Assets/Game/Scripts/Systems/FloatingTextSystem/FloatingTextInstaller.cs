using UnityEngine;

using Zenject;

namespace Game.Systems.FloatingTextSystem
{
	[CreateAssetMenu(menuName = "Installers/FloatingTextInstaller", fileName = "FloatingTextInstaller")]
	public class FloatingTextInstaller : ScriptableObjectInstaller<FloatingTextInstaller>
	{
		[SerializeField] private FloatingText floatingTextPrefab;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<FloatingSystem>().AsSingle();

			Container
				.BindFactory<FloatingText, FloatingText.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(3)
				.FromComponentInNewPrefab(floatingTextPrefab));
		}
	}
}