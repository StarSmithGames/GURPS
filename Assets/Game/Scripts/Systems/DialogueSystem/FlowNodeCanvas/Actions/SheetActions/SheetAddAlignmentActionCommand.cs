using Game.Systems.CommandCenter;
using Game.Systems.ContextMenu;

using NodeCanvas.DialogueTrees;

using ParadoxNotion.Design;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Systems.DialogueSystem.Nodes
{
	[Name("Add Alignment")]
	[Description("Is Use CommandSetAlignment")]
	[Category("\x2724 Sheet")]
	public class SheetAddAlignmentActionCommand : CommandActionTask
	{
		public bool usePercents = true;
		public AlignmentType type = AlignmentType.TrueNeutral;
		public float percentAlignment = 0;
		public Vector2 addAlignment = Vector2.zero;

		public override void Initialize()
		{
			var dt = ownerSystem as DialogueTree;
			if (dt != null)
			{
				var node = dt.CurrentNode;
				var sheet = (node.FinalActor as IActor)?.GetSheet();

				if (sheet != null)
				{
					if (usePercents)
					{
						Command = new CommandSetAlignment(sheet, type, percentAlignment);
					}
					else
					{
						Command = new CommandSetAlignment(sheet, addAlignment);
					}
				}
			}

			Assert.IsNotNull(Command, "Add Alignment command == null");
		}

		protected override void OnExecute()
		{
			Command?.Execute();

			EndAction(true);
		}

#if UNITY_EDITOR
		protected override void OnTaskInspectorGUI()
		{
			usePercents = EditorGUILayout.Toggle("Use Percents", usePercents);

			if (usePercents)
			{
				type = (AlignmentType)EditorGUILayout.EnumPopup("Type", type);
				percentAlignment = EditorGUILayout.FloatField("Percent Alignment", percentAlignment);
				percentAlignment = Mathf.Clamp(percentAlignment, 0.01f, 100f);
			}
			else
			{
				addAlignment = EditorGUILayout.Vector2Field("Add Alignment", addAlignment);
				addAlignment = new Vector2(Mathf.Clamp(addAlignment.x, -1, 1), Mathf.Clamp(addAlignment.y, -1, 1));
			}
		}
#endif
	}
}