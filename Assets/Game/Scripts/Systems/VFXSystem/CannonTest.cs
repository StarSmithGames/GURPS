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

	private ElectricBallProjectileVFX.Factory electricalBallFactory;

	[Inject]
	private void Construct(ElectricBallProjectileVFX.Factory electricalBallFactory)
	{
		this.electricalBallFactory = electricalBallFactory;
	}

	[Button]
	private void Fire()
	{
		var projectile = electricalBallFactory.Create();
		projectile
			.SetStart(point.position, point.forward)
			.SetTarget(target)
			.Launch();
	}
}