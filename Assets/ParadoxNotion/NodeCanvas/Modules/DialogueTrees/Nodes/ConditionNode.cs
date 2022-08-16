using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.DialogueTrees
{
    [ParadoxNotion.Design.Icon("Condition")]
    [Name("Task Condition")]
    [Category("Branch")]
    [Description("Execute the first child node if a Condition is true, or the second one if that Condition is false. The Actor selected is used for the Condition check")]
    [Color("b3ff7f")]
    public class ConditionNode : DTNode, ITaskAssignable<ConditionTask>
    {
        [SerializeField] private ConditionTask condition;

        public ConditionTask Condition { get => condition; set => condition = value; }
        public Task Task { get => Condition; set => Condition = (ConditionTask)value; }

        public override int maxOutConnections => 2;
        public override bool requireActorSelection => true;

        protected override Status OnExecute(Component agent, IBlackboard bb) {

            if ( outConnections.Count == 0 ) {
                return Error("There are no connections on the Dialogue Condition Node");
            }

            if ( Condition == null ) {
                return Error("There is no Conidition on the Dialoge Condition Node");
            }

            var isSuccess = Condition.CheckOnce(FinalActor.DialogueTransform, graphBlackboard);
            status = isSuccess ? Status.Success : Status.Failure;
            DLGTree.Continue(isSuccess ? 0 : 1);
            return status;
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        public override string GetConnectionInfo(int i) {
            return i == 0 ? "Then" : "Else";
        }

#endif
    }
}