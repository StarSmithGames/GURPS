using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion;
using ParadoxNotion.Design;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using I2.Loc;
#if UNITY_EDITOR
using UnityEditor;
#endif
[Name("I2Say")]
[Description("Make the selected Dialogue Actor talk. You can make the text more dynamic by using variable names in square brackets\ne.g. [myVarName] or [Global/myVarName]")]
public class I2StatementNode : DTNode
{
    [SerializeField]
    private Statement[] statements;

	public override bool requireActorSelection { get { return true; } }

    public Statement GetSubtitles()
	{
        return GetCurrentStatement();
    }

	public override void OnCreate(Graph assignedGraph)
	{
		var list = LocalizationManager.GetAllLanguages(true);
		statements = new Statement[list.Count];

		for (int i = 0; i < statements.Length; i++)
		{
			statements[i] = new Statement("This is a dialogue text");
		}

		base.OnCreate(assignedGraph);
	}

	protected override Status OnExecute(Component agent, IBlackboard bb)
    {
        var tempStatement = GetCurrentStatement()?.BlackboardReplace(bb);
        if (tempStatement == null) return Status.Error;

        DialogueTree.RequestSubtitles(new SubtitlesRequestInfo(finalActor, tempStatement, OnStatementFinish));
        return Status.Running;
    }

    private Statement GetCurrentStatement()
	{
        LocalizationManager.UpdateSources();
        int index = LocalizationManager.GetAllLanguages(true).IndexOf(LocalizationManager.CurrentLanguage);
        if (index >= 0 && index < statements.Length)
		{
            return statements[index];
        }
        return null;
    }

    void OnStatementFinish()
    {
        status = Status.Success;
        DLGTree.Continue();
    }

	///----------------------------------------------------------------------------------------------
	///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
	protected override void OnNodeInspectorGUI()
	{
        base.OnNodeInspectorGUI();

        LocalizationManager.UpdateSources();
        var list = LocalizationManager.GetAllLanguages(true);
        string languages = "Required: ";

        for (int i = 0; i < list.Count; i++)
		{
            languages += list[i] + (i < list.Count - 1 ? ", " : "");
        }

        EditorGUILayout.HelpBox(languages, MessageType.Warning);

        for (int i = 0; i < statements.Length; i++)
        {
            statements[i].OnGUI(list[i]);
        }

        GUILayout.Space(5f);
	}
	protected override void OnNodeGUI()
    {
        GUILayout.BeginVertical(Styles.roundedBox);
        GUILayout.Label("\"<i>" + (GetCurrentStatement()?.text.CapLength(30)?? "Empty") + "</i> \"");
        GUILayout.EndVertical();
    }
#endif
}