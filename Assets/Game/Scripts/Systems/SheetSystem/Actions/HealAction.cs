using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Systems.SheetSystem.Actions
{
	[CreateAssetMenu(fileName = "HealAction", menuName = "Game/Sheet/Actions/Heal")]
	public class HealAction : BaseAction
	{
		public float healAmount;

		public override void Execute(object target)
		{
			var sheet = (target as ISheetable).Sheet;

			Assert.IsNotNull(sheet);
		}
	}
}