using DG.Tweening;
using UnityEngine;

using Zenject;

namespace Game.Systems.VFX
{
	public class ElectricBallProjectileVFX : ProjectileVFX
	{
		[Space]
		[SerializeField] private ParticleSystem projectile;
		[SerializeField] private ParticleSystem explotion;
		[SerializeField] private Light light1;
		[SerializeField] private Light light2;
		[SerializeField] private TrailRenderer trail;

		public override void Launch()
		{
			base.Launch();

			projectile.Play(true);
			explotion.Stop(true);
		}

		protected override void Collision(RaycastHit hit)
		{
			base.Collision(hit);
			projectile.Stop(true);
			explotion.Play(true);

			Sequence sequence = DOTween.Sequence();
			sequence
				.Append(light1.DOIntensity(0, 0.15f))
				.Join(light2.DOIntensity(0, 0.15f))
				.AppendInterval(2.5f)
				.AppendCallback(() => DespawnIt());
		}

		public override void OnDespawned()
		{
			base.OnDespawned();

			projectile.Stop(true);
			explotion.Stop(true);

			trail.Clear();

			light1.intensity = 1f;
			light2.intensity = 2.25f;
		}

		public class Factory : PlaceholderFactory<ElectricBallProjectileVFX> { }
	}
}