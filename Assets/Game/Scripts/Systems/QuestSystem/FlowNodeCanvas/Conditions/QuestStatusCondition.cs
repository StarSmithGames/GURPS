using ParadoxNotion.Design;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.QuestSystem.Nodes
{
    [Name("Check Quest Status")]
    [Description("Check.")]
    [Category("\x2724 Quest")]
    public class QuestStatusCondition : QuestConditionTask
    {
        public QuestStatus required = QuestStatus.Success;
        public bool isLast = true;
    }
}