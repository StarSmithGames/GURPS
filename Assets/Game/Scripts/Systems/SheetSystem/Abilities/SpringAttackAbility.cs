using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.SheetSystem.Abilities
{
    public class SpringAttackAbility : AttackAbility
    {
        public override IActivation Activation => activationFactory.Create(ActivationType.Casted, this);
	}
}