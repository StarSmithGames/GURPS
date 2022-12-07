using Game.Systems.VFX;

using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class CannonTest : MonoBehaviour
{
	public Transform point;
	public Transform target;

	public TestVFX vfx;

	private ElectricBallProjectileVFX.Factory electricalBallFactory;

	[Inject]
	private void Construct(ElectricBallProjectileVFX.Factory electricalBallFactory)
	{
		this.electricalBallFactory = electricalBallFactory;
	}

	[Button]
	private void Fire()
	{
		if (vfx == TestVFX.ElectricBall)
		{
			var projectile = electricalBallFactory.Create();
			projectile
				.SetStart(point.position, point.forward)
				.SetTarget(target)
				.Launch();
		}
	}

	public enum TestVFX
	{
		ElectricBall,

	}
}