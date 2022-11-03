using System.Collections;

using UnityEngine;

namespace Game.Systems.InteractionSystem
{
	public class TaskWaitAttack : TaskAction
	{
		private AnimatorController.AnimatorController animatorController;

		public TaskWaitAttack(AnimatorController.AnimatorController animatorController)
		{
			this.animatorController = animatorController;
		}

		public override IEnumerator Implementation()
		{
			//wait start and then end attack
			yield return new WaitUntil(() => animatorController.IsAttackProccess);
			yield return new WaitUntil(() => !animatorController.IsAttackProccess);
		}
	}
}