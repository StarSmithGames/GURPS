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
        var tempStatement = GetStatement()?.BlackboardReplace(bb);
        if (tempStatement == null) return Status.Error;

        DialogueTree.RequestSubtitles(new SubtitlesRequestInfo(finalActor, tempStatement, OnStatementFinish));
        return Status.Running;
    }

    private Statement GetStatement()
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

            GUILayout.BeginVertical("box");
            Statement s = statements[i];
            GUILayout.Label(list[i]);

            GUIStyle style = new GUIStyle(EditorStyles.textArea);
            s.text = EditorGUILayout.TextArea(s.text, style);

            s.audio = EditorGUILayout.ObjectField("Audio File", s.audio, typeof(AudioClip), false) as AudioClip;
            s.meta = EditorGUILayout.TextField("Meta Data", s.meta);
            GUILayout.EndVertical();
        }

        GUILayout.Space(5f);
	}
	protected override void OnNodeGUI()
    {
        GUILayout.BeginVertical(Styles.roundedBox);
        GUILayout.Label("\"<i> " + (GetStatement()?.text.CapLength(30)?? "Empty") + "</i> \"");
        GUILayout.EndVertical();
    }
#endif
}