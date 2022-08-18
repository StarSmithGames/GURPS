using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion;
using ParadoxNotion.Design;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using I2.Loc;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Systems.DialogueSystem
{
    [Name("Say")]
    [Description("Make the selected Dialogue Actor talk. You can make the text more dynamic by using variable names in square brackets\ne.g. [myVarName] or [Global/myVarName]")]
    public class I2StatementNode : DTNode
    {
        [HideInInspector] public I2Texts<I2AudioText> statement;

        public bool waitForInput = true;

        public override bool requireActorSelection { get { return true; } }

        public override void OnCreate(Graph assignedGraph)
        {
            CheckStatement();
        }

        protected override Status OnExecute(Component agent, IBlackboard bb)
        {
            CheckStatement();
            var temp = statement.GetCurrent();
            if (temp == null) return Status.Error;

            DialogueTree.RequestSubtitles(new SubtitlesRequestInfo(FinalActor, temp, OnStatementFinish) { waitForInput = waitForInput });
            return Status.Running;
        }

        void OnStatementFinish()
        {
            status = Status.Success;
            DLGTree.Continue();
        }

        private void CheckStatement()
        {
            if (statement == null)
            {
                statement = new I2Texts<I2AudioText>();
            }
        }

#if UNITY_EDITOR
        protected override void OnNodeInspectorGUI()
        {
            LocalizationManager.LanguagesGUI();
            base.OnNodeInspectorGUI();

            CheckStatement();

            statement.OnGUI("Statement");
        }
        protected override void OnNodeGUI()
        {
            GUILayout.BeginVertical(Styles.roundedBox);
            GUILayout.Label("\"<i>" + (statement?.GetCurrent()?.value.CapLength(30) ?? "Empty") + "</i> \"");
            GUILayout.EndVertical();
        }
#endif
    }
}