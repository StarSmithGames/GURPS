using Game.Entities;
using Game.Systems.SheetSystem;

using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Game.Systems.DialogueSystem.Nodes
{
	public class SheetAddAlignmentCondition : ActionTask
	{
		public Alignment type = Alignment.TrueNeutral;
		public Vector2 alignment = Vector2.zero;

		protected override void OnExecute()
		{
			var dt = ownerSystem as DialogueTree;
			if (dt != null)
			{
				var node = dt.CurrentNode;
				var sheet = (node.FinalActor as IEntity)?.Sheet;

				if (sheet != null)
				{
					(sheet.Characteristics.Alignment as AlignmentCharacteristic).CurrentValue += alignment;
				}
			}

			EndAction(true);
		}

#if UNITY_EDITOR
		protected override void OnTaskInspectorGUI()
		{
			alignment = EditorGUILayout.Vector2Field("Alignment", alignment);
			alignment = new Vector2(Mathf.Clamp(alignment.x, -1, 1), Mathf.Clamp(alignment.y, -1, 1));
		}
#endif
	}
}