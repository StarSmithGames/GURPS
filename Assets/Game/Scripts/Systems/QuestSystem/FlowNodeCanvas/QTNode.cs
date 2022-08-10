using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion;

using System;
using UnityEngine;

namespace Game.Systems.QuestSystem.Nodes
{
	public abstract class QTNode : Node
	{
		public override int maxInConnections => -1;
		public override int maxOutConnections => 1;
		public override Type outConnectionType => typeof(QTConnection);
		public override bool allowAsPrime => true;
		public override bool canSelfConnect => false;
		public override Alignment2x2 commentsAlignment => Alignment2x2.Right;
		public override Alignment2x2 iconAlignment => Alignment2x2.Bottom;

		protected QuestTree QuestTree => (QuestTree)graph;
		protected Component Manager => AsyncManager.Instance;

#if UNITY_EDITOR
		protected override UnityEditor.GenericMenu OnContextMenu(UnityEditor.GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Breakpoint"), isBreakpoint, () => { isBreakpoint = !isBreakpoint; });
			return menu;
		}
#endif
	}
}