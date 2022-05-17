using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion.Design;

using UnityEngine;

[Name("IsChoiceSelectedFirstTime")]
[Description("Work only with Choices")]
public class IsChoiceSelectedFirstTimeConditionTask : ConditionTask
{
    protected override string info => "Choice Selected";

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