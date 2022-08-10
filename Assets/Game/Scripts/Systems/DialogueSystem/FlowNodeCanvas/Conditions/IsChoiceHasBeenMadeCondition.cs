using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion.Design;

using UnityEngine;

namespace Game.Systems.DialogueSystem.Nodes
{
    [Name("Is Choice Has Been Made")]
    [Description("Work only with Choices")]
    [Category("\x2724 Dialogue")]
    public class IsChoiceHasBeenMadeCondition : ConditionTask
    {
        protected override string info => "Choice Has Been Made";

        protected override bool OnCheck()
        {
            var dt = ownerSystem as DialogueTree;

            if (dt != null)
            {
                var node = dt.LastNode as I2MultipleChoiceNode;
                if (node != null)
                {
                    var choice = node.availableChoices[dt.LastNodeConnectionIndex];
                    return choice.isSelected;
                }
            }

            return false;
        }
    }
}