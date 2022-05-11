using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion;
using ParadoxNotion.Design;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using I2.Loc;
using Game.Systems.QuestSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestNode : DTNode
{
    public QuestData data;

#if UNITY_EDITOR
	protected override void OnNodeGUI()
	{
		GUILayout.BeginVertical(Styles.roundedBox);
		GUILayout.Label("\"<i>" + (data?.questName.CapLength(30) ?? "Empty") + "</i> \"");
		GUILayout.EndVertical();
	}
#endif
}