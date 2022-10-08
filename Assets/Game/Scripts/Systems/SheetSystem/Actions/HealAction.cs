using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Systems.SheetSystem.Actions
{
	public class HealAction : Action
	{
		public override void Execute(object target)
		{
			var sheet = (target as ISheetable).Sheet;

			Assert.IsNotNull(sheet);
		}
	}
}