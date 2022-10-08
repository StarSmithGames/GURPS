using Game.Systems.BattleSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Systems.AnimatorController
{
	public class SkeletonZombieAnimatorControl : AnimatorController
	{
		private IBattlable humanoid;

		public override void Initialize()
		{
			humanoid = characterModel as IBattlable;
		}

		protected override void Update()
		{
			base.Update();

			animator.SetBool(isBattleModeHash, humanoid.InBattle);

			if (humanoid.InBattle)
			{
				//humanoid.Controller.IsWaitAnimation = IsAnimationProcess;
			}
		}
	}
}