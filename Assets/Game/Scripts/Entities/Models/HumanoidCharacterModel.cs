using Game.Systems.AnimatorController;

using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Entities.Models
{
	public class HumanoidCharacterModel : CharacterModel
	{
		protected HumanoidAnimatorController animatorController;

		protected override IEnumerator Start()
		{
			yield return base.Start();

			animatorController = AnimatorController as HumanoidAnimatorController;

			animatorController.onAttack += OnAttacked;
			animatorController.onAttackLeftHand += OnAttackedLeftHand;
			animatorController.onAttackRightHand += OnAttackRightHand;
			animatorController.onAttackKick += OnAttackKick;
		}

		protected override void OnDestroy()
		{
			if (animatorController != null)
			{
				animatorController.onAttack -= OnAttacked;
				animatorController.onAttackLeftHand -= OnAttackedLeftHand;
				animatorController.onAttackRightHand -= OnAttackRightHand;
				animatorController.onAttackKick -= OnAttackKick;
			}

			base.OnDestroy();
		}

		private void OnAttacked()
		{
			Debug.LogError("OnAttacked");

			Assert.IsNotNull(currentCombat);

			currentCombat.DealDamage();
		}

		private void OnAttackedLeftHand()
		{
			Debug.LogError("OnAttackedLeftHand");
			OnAttacked();
		}

		private void OnAttackRightHand()
		{
			Debug.LogError("OnAttackRightHand");
			OnAttacked();
		}

		private void OnAttackKick()
		{
			Debug.LogError("OnAttackKick");
			OnAttacked();
		}
	}
}