using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem.Abilities
{
	public class AttackAbility : BaseAbility
	{
		public float damage = 1f;

		public override IActivation Activation => activationFactory.Create(ActivationType.Instant, this);
		

		[Inject]
		private void Construct()
		{

		}

		public override void Apply()
		{

		}
	}
}