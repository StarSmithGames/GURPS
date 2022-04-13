using Game.Systems.BattleSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonZombieAnimatorControl : AnimatorControl
{
	private IBattlable humanoid;

	protected override void Start()
	{
		humanoid = entity as IBattlable;
	}

	protected override void Update()
	{
		base.Update();

		animator.SetBool(isBattleModeHash, humanoid.InBattle);

		if (humanoid.InBattle)
		{
			humanoid.Controller.IsWaitAnimation = IsAnimationProcess;
		}
	}
}