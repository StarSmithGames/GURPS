using NodeCanvas.DialogueTrees;
using NodeCanvas.Editor;
using NodeCanvas.Framework;

using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Game.Systems.QuestSystem
{
	[CreateAssetMenu(fileName = "Quest", menuName = "Game/Quests/QuestTree")]
	public class QuestTree : DialogueTree
	{
		//public override Type baseNodeType => typeof(QuestNode);
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(QuestTree), true)]
	public class QuestTreeEditor : DialogueTreeInspector { }
#endif
}