using NodeCanvas.Framework;
using ParadoxNotion;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Systems.QuestSystem.Nodes
{
	public class QuestNode : QTNode
	{
		public QuestData data;

		public override string name
		{
			get
			{
				string result = data?.Title.CapLength(30) ?? "Quest";
				return string.IsNullOrEmpty(result) ? "Empty" : result;
			}
		}

#if UNITY_EDITOR
		protected override void OnNodeInspectorGUI()
		{
			data = (QuestData)EditorGUILayout.ObjectField("Data", data, typeof(QuestData), true);
		}
#endif
	}
}