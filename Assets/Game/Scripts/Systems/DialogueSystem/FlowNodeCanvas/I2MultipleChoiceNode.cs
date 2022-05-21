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

namespace Game.Systems.DialogueSystem.Nodes
{
    [ParadoxNotion.Design.Icon("List")]
    [Name("Multiple Choice")]
    [Category("Branch")]
    [Description("Prompt a Dialogue Multiple Choice. A choice will be available if the choice condition(s) are true or there is no choice conditions. The Actor selected is used for the condition checks and will also Say the selection if the option is checked.")]
    [Color("b3ff7f")]
    public class I2MultipleChoiceNode : DTNode
    {
        public override int maxOutConnections { get { return availableChoices.Count; } }
        public override bool requireActorSelection { get { return true; } }

        [SliderField(0f, 10f)]
        public float availableTime;
        [ShowIf("saySelection", 1)]
        public bool waitForInput = false;

        [SerializeField, AutoSortWithChildrenConnections]
        [HideInInspector] public List<ChoiceWrapper> availableChoices = new List<ChoiceWrapper>();
        private List<ChoiceWrapper> currentChoices = new List<ChoiceWrapper>();
        private IBlackboard cashedBB;

        protected override Status OnExecute(Component agent, IBlackboard bb)
        {
            cashedBB = bb;
            currentChoices.Clear();

            if (outConnections.Count == 0)
            {
                return Error("There are no connections to the Multiple Choice Node!");
            }

            availableChoices.ForEach((availableChoice) =>
            {
                if (availableChoice.conditionBefore != null)
				{
                    //conditionBefore check on ActorSheetCondition
                    if (IsActorSheetCondition(availableChoice.conditionBefore, out ActorSheetCondition actorSheetCondition))
                    {
                        if (actorSheetCondition.CheckOnce(FinalActor.Transform, bb))
                        {
                            availableChoice.choice.choiceConditionState = ChoiceConditionState.Normal;
                            currentChoices.Add(availableChoice);
                        }
                        else if (actorSheetCondition.state != ChoiceConditionState.Ignore)//Если не прошёл условие то игнорируем чойс или делаем не доступным.
                        {
                            availableChoice.choice.choiceConditionState = actorSheetCondition.state;
                            currentChoices.Add(availableChoice);
                        }
                    }
                    else if (availableChoice.conditionBefore.CheckOnce(FinalActor.Transform, bb))
                    {
                        availableChoice.choice.choiceConditionState = ChoiceConditionState.Normal;
                        currentChoices.Add(availableChoice);
                    }
                }
				else
				{
                    currentChoices.Add(availableChoice);
                }
            });

            AddStuff();

            var optionsInfo = new MultipleChoiceRequestInfo()
            {
                actor = FinalActor,
                choices = currentChoices.Select((x) => x.choice).ToList(),
                availableTime = availableTime,
                SelectOption = OnOptionSelected,
            };
            optionsInfo.showLastStatement = inConnections.Count > 0 && inConnections[0].sourceNode is StatementNode;
            DialogueTree.RequestMultipleChoices(optionsInfo);
            return Status.Running;
        }

        private void AddStuff()
        {
            currentChoices.ForEach((currentChoice) =>
            {
                //conditionBefore add requirements
                if (IsActorSheetCondition(currentChoice.conditionBefore, out ActorSheetCondition actorSheetCondition))
                {
                    if (actorSheetCondition.condition.Is<ConditionList>(out ConditionList list))
                    {
                        var conditionTasks = list.conditions.OfType<RequirementConditionTask>().ToList();
                        currentChoice.choice.consequence.AddRange(conditionTasks.Select((x) => x.requirement));
                    }
                    else
                    {
                        if (actorSheetCondition.condition is RequirementConditionTask requirementCondition)
                        {
                            currentChoice.choice.requirements.Add(requirementCondition.requirement);
                        }
                    }
                }

                //actionAfter initialize and select commands
                if (currentChoice.actionAfter != null)
                {
                    if (currentChoice.actionAfter.Is<ActionList>(out ActionList list))
                    {
                        var actionTasks = list.actions.OfType<CommandActionTask>().ToList();
                        actionTasks.ForEach((x) => x.Initialize());
                        currentChoice.choice.consequence.AddRange(actionTasks.Select((x) => x.command));
                    }
                    else
                    {
                        if (currentChoice.actionAfter is CommandActionTask commandAction)
                        {
                            commandAction.Initialize();
                            currentChoice.choice.consequence.Add(commandAction.command);
                        }
                    }
                }
            });
        }


        private bool IsActorSheetCondition(ConditionTask condition, out ActorSheetCondition actorSheetCondition)
        {
            actorSheetCondition = null;

            if (condition.Is<ConditionList>(out ConditionList list))
            {
                if (list.conditions.OfType<ActorSheetCondition>().Any())
                {
                    actorSheetCondition = list.conditions.Find((x) => x is ActorSheetCondition) as ActorSheetCondition;
                    return true;
                }
            }
            else if (condition.Is<ActorSheetCondition>(out actorSheetCondition))
            {
                return true;
            }

            return false;
        }


        private void OnOptionSelected(int index)
        {
            status = Status.Success;

            var choice = currentChoices[index];

            choice.actionAfter?.Execute(FinalActor.Transform, cashedBB);
            DLGTree.Continue(index);

            choice.choice.isSelected = true;

            currentChoices.ForEach((x) => x.choice.Dispose());
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
            if (availableChoices[i].conditionBefore == null)
            {
                return text;
            }
            return string.Format("{0}\n{1}", text, availableChoices[i].conditionBefore.summaryInfo);
        }

        protected override void OnNodeGUI()
        {
            if (availableChoices.Count == 0)
            {
                GUILayout.Label("No Options Available");
                return;
            }

            GUILayout.Label($"Choices Count {availableChoices.Count}");
            for (var i = 0; i < availableChoices.Count; i++)
            {
                var choice = availableChoices[i];
                var connection = i < outConnections.Count ? outConnections[i] : null;
                GUILayout.BeginHorizontal(Styles.roundedBox);
                GUILayout.Label(string.Format("{0} {1} {2}", connection != null ? "O" : "X", $"[{i + 1}]", choice.GetStatement().Text.CapLength(30)), Styles.leftLabel);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (availableTime > 0)
            {
                GUILayout.Label(availableTime + "' Seconds");
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
                    wrapper.choice.options.Add(new ChoiceOption($"I am a choice..."));
                }

                availableChoices.Add(wrapper);
            }

            if (availableChoices.Count == 0)
            {
                return;
            }

            GUILayout.Label($"Choices Count {availableChoices.Count}");
            EditorUtils.ReorderableList(availableChoices, (i, picked) =>
            {
                var choice = availableChoices[i];
                GUILayout.BeginHorizontal("box");

                var text = string.Format("{0} {1} {2}", choice.isShowFoldout ? "-" : "+", $"[{i + 1}]", choice.GetStatement().Text);
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

        public bool isShowFoldout = false;

        public ConditionTask conditionBefore;
        public ActionTask actionAfter;

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

            NodeCanvas.Editor.TaskEditor.TaskFieldMulti(conditionBefore, graph, (task) => { conditionBefore = task; }, postfix: " BEFORE");

            var list = LocalizationManager.GetAllLanguages(true);
            for (int i = 0; i < choice.options.Count; i++)
            {
                (choice.options[i].Statement as Statement).OnGUI(list[i]);
                GUILayout.Space(10);
            }

            NodeCanvas.Editor.TaskEditor.TaskFieldMulti(actionAfter, graph, (task) => { actionAfter = task; }, postfix: " AFTER");

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
    }
}