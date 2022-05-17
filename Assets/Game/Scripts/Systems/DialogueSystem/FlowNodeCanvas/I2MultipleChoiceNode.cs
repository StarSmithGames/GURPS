using I2.Loc;

using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion;
using ParadoxNotion.Design;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    [HideInInspector] public List<ChoiceWrapper> availableChoices = new List<ChoiceWrapper>();

    public override int maxOutConnections { get { return availableChoices.Count; } }
    public override bool requireActorSelection { get { return true; } }

    protected override Status OnExecute(Component agent, IBlackboard bb)
    {
        if (outConnections.Count == 0)
        {
            return Error("There are no connections to the Multiple Choice Node!");
        }

        var optionsInfo = new MultipleChoiceRequestInfo()
		{
            actor = finalActor,
            choices = availableChoices.Select((x) => x.choice).ToList(),
            availableTime = availableTime,
            SelectOption = OnOptionSelected,
        };
        optionsInfo.showLastStatement = inConnections.Count > 0 && inConnections[0].sourceNode is StatementNode;
        DialogueTree.RequestMultipleChoices(optionsInfo);
        return Status.Running;
    }

    private void OnOptionSelected(int index)
    {
        status = Status.Success;

        Action action = () =>
        {
            DLGTree.Continue(index);
            availableChoices[index].choice.isSelected = true;
        };

        if (saySelection)
        {
            //Персонаж проговаривает выбраную опцию
            var tempStatement = availableChoices[index].GetStatement().BlackboardReplace(graphBlackboard);
            var speechInfo = new SubtitlesRequestInfo(finalActor, tempStatement, action);
            DialogueTree.RequestSubtitles(speechInfo);
        }
        else
        {
            action?.Invoke();
        }
    }

#if UNITY_EDITOR
    public override void OnConnectionInspectorGUI(int i)
    {
        availableChoices[i].OnGUI(graph);
    }

    public override string GetConnectionInfo(int i)
    {
        if (i >= availableChoices.Count)
        {
            return "NOT SET";
        }
        var text = string.Format("'{0}'", availableChoices[i].GetStatement().Text);
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
            GUILayout.Label(string.Format("{0} {1}", connection != null ? "O" : "X", choice.GetStatement().Text.CapLength(30)), Styles.leftLabel);
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
            ChoiceWrapper wrapper = new ChoiceWrapper();

            for (int i = 0; i < list.Count; i++)
            {
                wrapper.choice.options.Add(new ChoiceOption("I am a choice..."));
            }

            availableChoices.Add(wrapper);
        }

        if (availableChoices.Count == 0)
        {
            return;
        }

        EditorUtils.ReorderableList(availableChoices, (i, picked) =>
        {
            var choice = availableChoices[i];
            GUILayout.BeginHorizontal("box");

            var text = string.Format("{0} {1}", choice.isShowFoldout ? "-" : "+", choice.GetStatement().Text);
            if (GUILayout.Button(text, (GUIStyle)"label", GUILayout.Width(0), GUILayout.ExpandWidth(true)))
            {
                choice.isShowFoldout = !choice.isShowFoldout;
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

            if (choice.isShowFoldout)
            {
                choice.OnGUI(graph);
            }
        });

    }
#endif
}

public class ChoiceWrapper
{
    public Choice choice;

    public bool isShowFoldout = true;

    public ConditionTask condition;

    public ChoiceWrapper()
	{
        choice = new Choice();
    }

    public Statement GetStatement()
    {
        int index = LocalizationManager.GetAllLanguages(true).IndexOf(LocalizationManager.CurrentLanguage);
        if (index >= 0 && index < choice.options.Count)
        {
            return choice.options[index].Statement as Statement;
        }
        return null;
    }


    public void OnGUI(Graph graph)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.BeginVertical("box");

        var list = LocalizationManager.GetAllLanguages(true);
        for (int i = 0; i < choice.options.Count; i++)
        {
            (choice.options[i].Statement as Statement).OnGUI(list[i]);
            GUILayout.Space(10);
        }

        NodeCanvas.Editor.TaskEditor.TaskFieldMulti<ConditionTask>(condition, graph, (choice) => { condition = choice; });

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
    }
}