using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Game.Systems.QuestSystem
{
	[CreateAssetMenu(menuName = "Game/Quests/QuestGroup", fileName = "QuestGroup")]
	public class QuestGroupData : ScriptableObject
	{
		[HideInInspector] public I2Texts<I2Text> questTitle;
		public List<QuestTree> quests = new List<QuestTree>();
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(QuestGroupData))]
	public class QuestGroupDataEditor : Editor
	{
		private QuestGroupData QuestGroup => (QuestGroupData)target;

		public override void OnInspectorGUI()
		{
			QuestGroup.questTitle.OnGUI("Title");
			DrawPropertiesExcluding(serializedObject, "m_Script");
		}
	}
#endif
}