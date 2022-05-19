using Game.Entities;
using Game.Systems.SheetSystem;

using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace Game.Systems.DialogueSystem.Nodes
{
	[Name("Check Alignment")]
	[Description("Check Current Actor Alignment.")]
	[Category("\x2724 Sheet")]
	public class SheetCheckAlignmentCondition : ConditionTask
	{
		public Alignment alignmentRequired = Alignment.TrueNeutral;

		protected override bool OnCheck()
		{
			var dt = ownerSystem as DialogueTree;
			if (dt != null)
			{
				var node = dt.CurrentNode;

				var sheet = (node.FinalActor as IEntity)?.Sheet;
			
				if(sheet != null)
				{
					return (sheet.Characteristics.Alignment as AlignmentCharacteristic).Aligment == alignmentRequired;
				}
			}

			return false;
		}
	}
}