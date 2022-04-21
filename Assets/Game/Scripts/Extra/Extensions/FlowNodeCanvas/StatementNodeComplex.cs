using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion;
using ParadoxNotion.Design;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Name("SayComplex")]
[Description("Make the selected Dialogue Actor talk. You can make the text more dynamic by using variable names in square brackets\ne.g. [myVarName] or [Global/myVarName]")]
public class StatementNodeComplex : DTNode
{
    [SerializeField]
    public Statement statement = new Statement("This is a dialogue text");

    public override bool requireActorSelection { get { return true; } }

    protected override Status OnExecute(Component agent, IBlackboard bb)
    {
        var tempStatement = statement.BlackboardReplace(bb);
        DialogueTree.RequestSubtitles(new SubtitlesRequestInfo(finalActor, tempStatement, OnStatementFinish));
        return Status.Running;
    }

    void OnStatementFinish()
    {
        status = Status.Success;
        DLGTree.Continue();
    }

    ///----------------------------------------------------------------------------------------------
    ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
    protected override void OnNodeGUI()
    {
        GUILayout.BeginVertical(Styles.roundedBox);
        GUILayout.Label("\"<i> " + statement.text.CapLength(30) + "</i> \"");
        GUILayout.EndVertical();
    }
#endif
}