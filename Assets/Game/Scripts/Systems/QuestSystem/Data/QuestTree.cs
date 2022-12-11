using Game.Systems.QuestSystem.Nodes;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System;

#if UNITY_EDITOR
using NodeCanvas.Editor;
using UnityEditor;
#endif

using UnityEngine;

namespace Game.Systems.QuestSystem
{
	[CreateAssetMenu(fileName = "Quest", menuName = "Game/Quests/QuestTree")]
	public partial class QuestTree : Graph
	{
		public I2Texts<I2Text> questTitle;
		public I2Texts<I2Text> questDescription;

		public static QuestTree currentDialogue { get; private set; }
		public static QuestTree previousDialogue { get; private set; }

		public QTNode CurrentNode { get; private set; }
		public QTNode LastNode { get; private set; }
		public int LastNodeConnectionIndex { get; private set; }


		public override Type baseNodeType => typeof(QTNode);
		public override bool requiresAgent => false;
		public override bool requiresPrimeNode => true;
		public override bool isTree => true;
		public override bool allowBlackboardOverrides => true;
		sealed public override bool canAcceptVariableDrops => false;

		public void Continue(int index = 0)
		{
			if (index < 0 || index > CurrentNode.outConnections.Count - 1)
			{
				Stop(true);
				return;
			}
			CurrentNode.outConnections[index].status = Status.Success; //editor vis
			LastNodeConnectionIndex = index;
			EnterNode((QTNode)CurrentNode.outConnections[index].targetNode);
		}

		///<summary>Enters the provided node</summary>
		public void EnterNode(QTNode node)
		{
			LastNode = CurrentNode;
			CurrentNode = node;
			CurrentNode.Reset(false);
			if (CurrentNode.Execute(agent, blackboard) == Status.Error)
			{
				Stop(false);
			}
		}

#if UNITY_EDITOR
		[UnityEditor.MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/Quest Tree Object", false, 2)]
		static void Editor_CreateGraph()
		{
			var dt = new GameObject("QuestTree").AddComponent<QuestTreeController>();
			UnityEditor.Selection.activeObject = dt;
		}
#endif
	}

	//Actions
	public partial class QuestTree
	{
		public static event Action<QuestTree> OnDialogueStarted;
		public static event Action<QuestTree> OnDialoguePaused;
		public static event Action<QuestTree> OnDialogueFinished;
		//public static event Action<SubtitlesRequestInfo> OnSubtitlesRequest;
		//public static event Action<MultipleChoiceRequestInfo> OnMultipleChoiceRequest;

		protected override void OnGraphStarted()
		{
			previousDialogue = currentDialogue;
			currentDialogue = this;

			//Logger.Log(string.Format("Dialogue Started '{0}'", this.name), "Dialogue Tree", this);
			if (OnDialogueStarted != null)
			{
				OnDialogueStarted(this);
			}

			if (!(agent is IDialogueActor))
			{
				//Logger.Log("Agent used in DialogueTree does not implement IDialogueActor. A dummy actor will be used.", "Dialogue Tree", this);
			}

			CurrentNode = CurrentNode != null ? CurrentNode : (QTNode)primeNode;
			EnterNode(CurrentNode);
		}

		protected override void OnGraphUpdate()
		{
			if (CurrentNode is IUpdatable updatable)
			{
				updatable.Update();
			}
		}

		protected override void OnGraphStoped()
		{
			currentDialogue = previousDialogue;
			previousDialogue = null;
			CurrentNode = null;

			//Logger.Log(string.Format("Dialogue Finished '{0}'", this.name), "Dialogue Tree", this);
			if (OnDialogueFinished != null)
			{
				OnDialogueFinished(this);
			}
		}

		protected override void OnGraphPaused()
		{
			//Logger.Log(string.Format("Dialogue Paused '{0}'", this.name), "Dialogue Tree", this);
			if (OnDialoguePaused != null)
			{
				OnDialoguePaused(this);
			}
		}

		protected override void OnGraphUnpaused()
		{
			CurrentNode = CurrentNode != null ? CurrentNode : (QTNode)primeNode;
			EnterNode(CurrentNode);

			//Logger.Log(string.Format("Dialogue Resumed '{0}'", this.name), "Dialogue Tree", this);
			if (OnDialogueStarted != null)
			{
				OnDialogueStarted(this);
			}
		}
	}


#if UNITY_EDITOR
	[CustomEditor(typeof(QuestTree), true)]
	public class QuestTreeEditor : GraphInspector
	{
		private QuestTree quest => target as QuestTree;

		public override void OnInspectorGUI()
		{
			quest.questTitle.OnGUI("Quest Title");
			quest.questDescription.OnGUI("Quest Description", true);
			base.OnInspectorGUI();
			EditorUtils.EndOfInspector();
			if (GUI.changed) { UndoUtility.SetDirty(quest); }
		}
	}
#endif
}