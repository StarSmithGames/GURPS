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
		public AlignmentType alignmentRequired = AlignmentType.TrueNeutral;

		protected override bool OnCheck()
		{
			var dt = ownerSystem as DialogueTree;
			if (dt != null)
			{
				var node = dt.CurrentNode;

				var sheet = (node.FinalActor as IEntity)?.Sheet;

				if (sheet != null)
				{
					Requirement = new AlignmentRequirement(sheet) { alignmentRequired = alignmentRequired };
				}
			}

			Assert.IsNotNull(Requirement, "Alignment requirement == null");

			return Requirement?.Check() ?? false;
		}
	}
}