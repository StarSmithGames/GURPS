using I2.Loc;

using ModestTree;

using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion;
using ParadoxNotion.Design;

using UnityEditor;

using UnityEngine;

[ParadoxNotion.Design.Icon("Condition")]
[Name("Task Statement Condition")]
[Category("Branch")]
[Description("Execute the first child node if a Condition is true, or the second one if that Condition is false. The Actor selected is used for the Condition check")]
[Color("b3ff7f")]
public class I2StatementConditionNode : ConditionNode
{
    [SerializeField] private bool saySelection = false;
    [SerializeField] private Statements statementsTrue;
    [SerializeField] private Statements statementsFalse;

    private bool isTrue = true;

    public override void OnCreate(Graph assignedGraph)
    {
        var list = LocalizationManager.GetAllLanguages(true);
        statementsTrue = new Statements(list);
        statementsFalse = new Statements(list);

        base.OnCreate(assignedGraph);
    }

	protected override Status OnExecute(Component agent, IBlackboard bb)
	{
        if (outConnections.Count == 0)
        {
            return Error("There are no connections on the Dialogue Condition Node");
        }

        if (Condition == null)
        {
            return Error("There is no Conidition on the Dialoge Condition Node");
        }

        isTrue = Condition.CheckOnce(finalActor.Transform, graphBlackboard);
        var statement = (isTrue? statementsTrue : statementsFalse).statements[LocalizationManager.CurrentLanguageIndex]?.BlackboardReplace(bb);
        if (statement == null) return Status.Error;

        DialogueTree.RequestSubtitles(new SubtitlesRequestInfo(finalActor, statement, OnStatementFinish));

        return Status.Running;
    }

    private void OnStatementFinish()
    {
        status = Status.Success;
        DLGTree.Continue(isTrue ? 0 : 1);
    }

    private string GetLabel(Statements statements)
	{
        return statements.GetCurrentStatementLabel(LocalizationManager.CurrentLanguageIndex);
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

        statementsTrue.OnGUI("True", LocalizationManager.CurrentLanguage, list);
        statementsFalse.OnGUI("False", LocalizationManager.CurrentLanguage, list);

        GUILayout.Space(10f);
    }
    protected override void OnNodeGUI()
	{
		GUILayout.BeginVertical(Styles.roundedBox);
		GUILayout.Label($"<i>{GetLabel(statementsTrue)}\n?\n{GetLabel(statementsFalse)}</i>");
		GUILayout.EndVertical();
	}
#endif
}