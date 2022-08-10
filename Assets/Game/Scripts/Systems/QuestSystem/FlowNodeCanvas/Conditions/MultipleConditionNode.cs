using NodeCanvas.Framework;

using ParadoxNotion.Design;

using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.QuestSystem.Nodes
{
    [ParadoxNotion.Design.Icon("Selector")]
    [Name("Multiple Task Condition")]
    [Category("Branch")]
    [Description("Will continue with the first child node which condition returns true. The Dialogue Actor selected will be used for the checks")]
    [Color("b3ff7f")]
    public class MultipleConditionNode : QTNode
    {
        [SerializeField, AutoSortWithChildrenConnections] private List<ConditionTask> conditions = new List<ConditionTask>();

        public override int maxOutConnections => -1;

        protected override Status OnExecute(Component agent, IBlackboard bb)
        {
            if (outConnections.Count == 0)
            {
                return Error("There are no connections on the Dialogue Condition Node");
            }

            for (var i = 0; i < outConnections.Count; i++)
            {
                if (conditions[i] == null || conditions[i].CheckOnce(Manager, graphBlackboard))
                {
                    QuestTree.Continue(i);
                    return Status.Success;
                }
            }

            ParadoxNotion.Services.Logger.LogWarning("No condition is true. Dialogue Ends.", LogTag.EXECUTION, this);
            QuestTree.Stop(false);
            return Status.Failure;
        }


        public override void OnChildConnected(int index)
        {
            if (conditions.Count < outConnections.Count)
            {
                conditions.Insert(index, null);
            }
        }

        public override void OnChildDisconnected(int index)
        {
            conditions.RemoveAt(index);
        }


#if UNITY_EDITOR
        public override void OnConnectionInspectorGUI(int i)
        {
            NodeCanvas.Editor.TaskEditor.TaskFieldMulti<ConditionTask>(conditions[i], QuestTree, (c) => { conditions[i] = c; });
        }

        public override string GetConnectionInfo(int i)
        {
            return conditions[i] != null ? conditions[i].summaryInfo : "TRUE";
        }
#endif
    }
}