using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion.Design;

using UnityEngine;

namespace Game.Systems.DialogueSystem.Nodes
{
    [Name("Is Choice Has Been Made")]
    [Description("Work only with Choices")]
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
                    var choiceWrapper = node.availableChoices[dt.LastNodeConnectionIndex];
                    return choiceWrapper.choice.isSelected;
                }
            }

            return false;
        }
    }
}