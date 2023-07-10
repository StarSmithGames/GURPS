using FlowCanvas.Nodes;

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
        public override int maxOutConnections => availableChoices.Count;
        public override bool requireActorSelection => true;

        [SliderField(0f, 10f)]
        public float availableTime;

        [SerializeField, AutoSortWithChildrenConnections]
        [HideInInspector] public List<Choice> availableChoices = new List<Choice>();
        private List<Choice> currentChoices = new List<Choice>();
        private IBlackboard cachedBB;

		protected override Status OnExecute(Component agent, IBlackboard bb)
        {
            cachedBB = bb;
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
                        if (actorSheetCondition.CheckOnce(FinalActor.DialogueTransform, bb))
                        {
                            availableChoice.choiceConditionState = ChoiceConditionState.Normal;
                            currentChoices.Add(availableChoice);
                        }
                        else if (actorSheetCondition.state != ChoiceConditionState.Ignore)//���� �� ������ ������� �� ���������� ���� ��� ������ �� ���������.
                        {
                            availableChoice.choiceConditionState = actorSheetCondition.state;
                            currentChoices.Add(availableChoice);
                        }
                    }
                    else if (availableChoice.conditionBefore.CheckOnce(FinalActor.DialogueTransform, bb))
                    {
                        availableChoice.choiceConditionState = ChoiceConditionState.Normal;
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
				choices = currentChoices.Select((x) => x as IChoice).ToList(),
				availableTime = availableTime,
                SelectOption = OnOptionSelected,
            };

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
                        currentChoice.consequence.AddRange(conditionTasks.Select((x) => x.Requirement));
                    }
                    else
                    {
                        if (actorSheetCondition.condition is RequirementConditionTask requirementCondition)
                        {
                            currentChoice.requirements.Add(requirementCondition.Requirement);
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
						currentChoice.consequence.AddRange(actionTasks.Select((x) => x.Command));
					}
                    else
                    {
                        if (currentChoice.actionAfter is CommandActionTask commandAction)
                        {
                            commandAction.Initialize();
							currentChoice.consequence.Add(commandAction.Command);
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
            choice.actionAfter?.Execute(FinalActor.DialogueTransform, cachedBB);
            choice.isSelected = true;
            currentChoices.ForEach((x) => x.Dispose());

            DLGTree.Continue(index);
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
			var text = string.Format("'{0}{1}'", $"[{i + 1}] ", availableChoices[i].statement.GetCurrent().Text);
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
				GUILayout.Label(string.Format("{0} {1} {2}", connection != null ? "O" : "X", $"[{i + 1}]", choice.statement.GetCurrent().Text.CapLength(30)), Styles.leftLabel);
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

			if (GUILayout.Button("Add Choice"))
			{
                var choice = new Choice();
				choice.statement = new I2Texts<I2AudioText>();
                availableChoices.Add(choice);
			}
            if (availableChoices.Count == 0)
            {
                return;
            }

            GUILayout.Label($"Choices Count {availableChoices.Count}");

            EditorUtils.ReorderableList(availableChoices, (i, picked) =>
            {
                var choice = availableChoices[i];
                bool lastFoldout = choice.isShowFoldout;

                GUILayout.BeginHorizontal("box");
                var text = string.Format("{0} {1} {2}", choice.isShowFoldout ? "-" : "+", $"[{i + 1}]", choice.statement.GetCurrent().Text);
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
				else
				{
                    choice.statement.isShowFoldout = false;
                }
			});

		}
#endif
    }


    [System.Serializable]
    public class Choice : IChoice// >:c
    {
        public string Text { get; }

        public bool isSelected = false;

        public List<object> requirements = new List<object>();
        public List<object> consequence = new List<object>();
        public List<object> actions = new List<object>();

        public ChoiceConditionState choiceConditionState = ChoiceConditionState.Normal;

        public I2Texts<I2AudioText> statement;

        public ConditionTask conditionBefore;
        public ActionTask actionAfter;

        public Data GetData()
        {
            return new Data()
            {
                isSelected = isSelected,
            };
        }


        public void Dispose()
        {
            requirements.Clear();
            consequence.Clear();
            actions.Clear();
        }

        public class Data
        {
            public bool isSelected;
        }


#if UNITY_EDITOR
        public bool isShowFoldout = false;

        public void OnGUI(Graph graph)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            GUILayout.BeginVertical("box");

            NodeCanvas.Editor.TaskEditor.TaskFieldMulti(conditionBefore, graph, (task) => { conditionBefore = task; }, postfix: " BEFORE");
            statement.OnGUI("Statement");
            NodeCanvas.Editor.TaskEditor.TaskFieldMulti(actionAfter, graph, (task) => { actionAfter = task; }, postfix: " AFTER");

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
#endif
    }

    public enum ChoiceConditionState
    {
        Normal,
        Inactive,
        Unavailable,
        Reason,
        Ignore,
    }
}