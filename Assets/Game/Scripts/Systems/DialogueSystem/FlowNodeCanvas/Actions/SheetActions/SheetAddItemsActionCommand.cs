using Game.Entities;
using Game.Systems.ContextMenu;
using Game.Systems.InventorySystem;

using NodeCanvas.DialogueTrees;

using ParadoxNotion.Design;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Systems.DialogueSystem.Nodes
{
	[Name("Add Item")]
	[Description("Is Use CommandAddItems")]
	[Category("\x2724 Sheet")]
	public class SheetAddItemsActionCommand : CommandActionTask
	{
		[SerializeField] public List<Item> items = new List<Item>();

		public override void Initialize()
		{
			var dt = ownerSystem as DialogueTree;
			if (dt != null)
			{
				var node = dt.CurrentNode;

				var sheet = (node.FinalActor as IActor)?.GetSheet();

				if (sheet != null)
				{
					Command = new CommandAddItems(sheet, items);
				}
			}

			Assert.IsNotNull(Command, "Add Items Command == null");
		}

		protected override void OnExecute()
		{
			Command?.Execute();

			EndAction(true);
		}

#if UNITY_EDITOR
		protected override void OnTaskInspectorGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginVertical();
			Item.OnGUIList(items);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
#endif
	}
}