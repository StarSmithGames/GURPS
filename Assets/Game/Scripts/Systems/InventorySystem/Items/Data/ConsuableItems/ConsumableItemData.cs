using Sirenix.OdinInspector;
using UnityEngine;
using Game.Systems.SheetSystem;
using Game.Systems.QuestSystem;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

using System.Collections.Generic;


namespace Game.Systems.InventorySystem
{
    public abstract class ConsumableItemData : ItemData
    {
		[SerializeReference]
		public List<Effect> effects = new List<Effect>();
	}

//#if UNITY_EDITOR
//	[CustomEditor(typeof(ConsumableItemData), true)]
//    public class ConsumableItemDataEditor : OdinEditor
//	{
//		private PropertyTree tree;
//		private ConsumableItemData data;

//		public override void OnInspectorGUI()
//		{
//			tree = this.Tree;
//			data = this.target as ConsumableItemData;

//			base.OnInspectorGUI();

//			tree.BeginDraw(true);

//			if (GUILayoutExtensions.DrawBigButton("Assign Effect", 28))
//			{
//				Search.OpenSearch<Effect>((obj) =>
//				{
//					Effect effect = (Effect)obj;
//					data.effects.Add(effect);

//					tree.ApplyChanges();
//					Undo.RegisterCompleteObjectUndo(data, $"Assign Effect");
//					serializedObject.Update();
//				});
//			}

//			serializedObject.ApplyModifiedProperties();
//			tree.EndDraw();
//		}
//	}
//#endif
}