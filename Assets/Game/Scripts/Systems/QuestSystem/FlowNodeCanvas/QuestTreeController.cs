using Game.Systems.QuestSystem;

using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTreeController : GraphOwner<QuestTree>, IDialogueActor
{
	public Transform DialogueTransform => transform;

    public void StartQuest(IDialogueActor instigator, Action<bool> callback)
    {
        //graph = GetInstance(graph);
        //graph.StartGraph(instigator is Component ? (Component)instigator : instigator.Transform, blackboard, updateMode, callback);
    }

#if UNITY_EDITOR
    new void Reset()
    {
        base.enableAction = EnableAction.DoNothing;
        base.disableAction = DisableAction.DoNothing;
        blackboard = gameObject.GetAddComponent<Blackboard>();
        SetBoundGraphReference(ScriptableObject.CreateInstance<QuestTree>());
    }
#endif
}