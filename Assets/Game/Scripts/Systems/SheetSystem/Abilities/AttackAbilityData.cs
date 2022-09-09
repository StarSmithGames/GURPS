using UnityEngine;

namespace Game.Systems.SheetSystem.Abilities
{
	[CreateAssetMenu(fileName = "AttackAbilityData", menuName = "Game/Sheet/Abilities/Attack")]
	public class AttackAbilityData : AbilityData
	{
		[Space]
		public AttackAbility ability;

		public override IAbility Copy() => new AttackAbility(ability, this);
	}
}