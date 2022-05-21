using Game.Entities;
using Game.Systems.SheetSystem;

using NodeCanvas.DialogueTrees;

using ParadoxNotion.Design;

using UnityEngine.Assertions;

namespace Game.Systems.DialogueSystem.Nodes
{
	[Name("Check Alignment")]
	[Description("Check Current Actor Alignment.")]
	[Category("\x2724 Sheet")]
	public class SheetAlignmentRequirement : RequirementConditionTask
	{
		public Alignment alignmentRequired = Alignment.TrueNeutral;

		protected override bool OnCheck()
		{
			var dt = ownerSystem as DialogueTree;
			if (dt != null)
			{
				var node = dt.CurrentNode;

				var sheet = (node.FinalActor as IEntity)?.Sheet;

				if (sheet != null)
				{
					requirement = new AlignmentRequirement(sheet) { alignmentRequired = alignmentRequired };
				}
			}

			Assert.IsNotNull(requirement, "Alignment requirement == null");

			return requirement?.Check() ?? false;
		}
	}
}