using I2.Loc;

using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion;
using ParadoxNotion.Design;

using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

[ParadoxNotion.Design.Icon("List")]
[Name("I2Multiple Choice")]
[Category("Branch")]
[Description("Prompt a Dialogue Multiple Choice. A choice will be available if the choice condition(s) are true or there is no choice conditions. The Actor selected is used for the condition checks and will also Say the selection if the option is checked.")]
[Color("b3ff7f")]
public class I2MultipleChoiceNode : DTNode
{
    [SliderField(0f, 10f)]
    public float availableTime;
    public bool saySelection;

    [SerializeField, AutoSortWithChildrenConnections]
    private List<Choice> availableChoices = new List<Choice>();

    public override int maxOutConnections { get { return availableChoices.Count; } }
    public override bool requireActorSelection { get { return true; } }

    protected override Status OnExecute(Component agent, IBlackboard bb)
    {
        if (outConnections.Count == 0)
        {
            return Error("There are no connections to the Multiple Choice Node!");
        }

        var finalOptions = new Dictionary<IStatement, int>();

        for (var i = 0; i < availableChoices.Count; i++)
        {
            var condition = availableChoices[i].condition;
            if (condition == null || condition.CheckOnce(finalActor.transform, bb))
            {
                var tempStatement = availableChoices[i].GetStatement().BlackboardReplace(bb);
                finalOptions[tempStatement] = i;
            }
        }

        if (finalOptions.Count == 0)
        {
            ParadoxNotion.Services.Logger.Log("Multiple Choice Node has no available options. Dialogue Ends.", LogTag.EXECUTION, this);
            DLGTree.Stop(false);
            return Status.Failure;
        }

        var optionsInfo = new MultipleChoiceRequestInfo(finalActor, finalOptions, availableTime, OnOptionSelected);
        optionsInfo.showLastStatement = inConnections.Count > 0 && inConnections[0].sourceNode is StatementNode;
        DialogueTree.RequestMultipleChoices(optionsInfo);
        return Status.Running;
    }

    void OnOptionSelected(int index)
    {

        status = Status.Success;

        System.Action Finalize = () => { DLGTree.Continue(index); };

        if (saySelection)
        {
            var tempStatement = availableChoices[index].GetStatement().BlackboardReplace(graphBlackboard);
            var speechInfo = new SubtitlesRequestInfo(finalActor, tempStatement, Finalize);
            DialogueTree.RequestSubtitles(speechInfo);
        }
        else
        {
            Finalize();
        }
    }

#if UNITY_EDITOR
    public override void OnConnectionInspectorGUI(int i)
    {
        DoChoiceGUI(availableChoices[i]);
    }

    public override string GetConnectionInfo(int i)
    {
        if (i >= availableChoices.Count)
        {
            return "NOT SET";
        }
        var text = string.Format("'{0}'", availableChoices[i].GetStatement().text);
        if (availableChoices[i].condition == null)
        {
            return text;
        }
        return string.Format("{0}\n{1}", text, availableChoices[i].condition.summaryInfo);
    }

    protected override void OnNodeGUI()
    {
        if (availableChoices.Count == 0)
        {
            GUILayout.Label("No Options Available");
            return;
        }

        for (var i = 0; i < availableChoices.Count; i++)
        {
            var choice = availableChoices[i];
            var connection = i < outConnections.Count ? outConnections[i] : null;
            GUILayout.BeginHorizontal(Styles.roundedBox);
            GUILayout.Label(string.Format("{0} {1}", connection != null ? "O" : "X", choice.GetStatement().text.CapLength(30)), Styles.leftLabel);
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        if (availableTime > 0)
        {
            GUILayout.Label(availableTime + "' Seconds");
        }
        if (saySelection)
        {
            GUILayout.Label("Say Selection");
        }
        GUILayout.EndHorizontal();
    }

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

        //only choice

        if (GUILayout.Button("Add Choice"))
        {
            Choice choice = new Choice();

            for (int i = 0; i < list.Count; i++)
            {
                choice.statements.Add(new Statement("I am a choice..."));
            }

            availableChoices.Add(choice);
        }

        if (availableChoices.Count == 0)
        {
            return;
        }

        EditorUtils.ReorderableList(availableChoices, (i, picked) =>
        {
            var choice = availableChoices[i];
            GUILayout.BeginHorizontal("box");

            var text = string.Format("{0} {1}", choice.isUnfolded ? "-" : "+", choice.GetStatement().text);
            if (GUILayout.Button(text, (GUIStyle)"label", GUILayout.Width(0), GUILayout.ExpandWidth(true)))
            {
                choice.isUnfolded = !choice.isUnfolded;
            }

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                availableChoices.RemoveAt(i);
                if (i < outConnections.Count)
                {
                    graph.RemoveConnection(outConnections[i]);
                }
            }

            GUILayout.EndHorizontal();

            if (choice.isUnfolded)
            {
                DoChoiceGUI(choice);
            }
        });

    }

    void DoChoiceGUI(Choice c)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginVertical("box");

        var list = LocalizationManager.GetAllLanguages(true);

        GUILayout.Space(10);
        for (int i = 0; i < c.statements.Count; i++)
		{
            GUILayout.BeginVertical("box");
            Statement s = c.statements[i];
            GUILayout.Label(list[i]);
            s.text = EditorGUILayout.TextField(s.text);
            s.audio = EditorGUILayout.ObjectField("Audio File", s.audio, typeof(AudioClip), false) as AudioClip;
            s.meta = EditorGUILayout.TextField("Meta Data", s.meta);
            GUILayout.EndVertical();
        }

        NodeCanvas.Editor.TaskEditor.TaskFieldMulti<ConditionTask>(c.condition, graph, (choice) => { c.condition = choice; });

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
    }

#endif
}
[System.Serializable]
public class Choice
{
    public bool isUnfolded = true;

    public List<Statement> statements = new List<Statement>();

    public ConditionTask condition;

    public Statement GetStatement()
    {
        int index = LocalizationManager.GetAllLanguages(true).IndexOf(LocalizationManager.CurrentLanguage);
        if (index >= 0 && index < statements.Count)
        {
            return statements[index];
        }
        return null;
    }
}