using Game.Entities;

using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;

using ParadoxNotion.Design;

using UnityEngine;

namespace Game.Systems.DialogueSystem.Nodes
{
	[Name("Check Actor Sheet")]
	[Description("Work only with Choices.\n��� ������ ������� choice �����:\nNone-������ �� ����������. Inactive-������ ������������(�����).\nUnavailable-�����������(�������). Reason-�����������(�������) � ��������� �������.\nIgnore-������������.")]
	[Category("\x2724 Dialogue")]
	public class ActorSheetCondition : ConditionTask
	{
		public ConditionTask condition;

		public ChoiceConditionState state = ChoiceConditionState.Inactive;

		protected override string info => $"(CheckActorSheet)\nElse {state}";

		protected override bool OnCheck()
		{
			var dt = ownerSystem as DialogueTree;
			if (dt != null)
			{
				//var node = dt.CurrentNode;
				//var sheet = (node.FinalActor as IEntityModel)?.Sheet;

				//if (sheet != null)
				//{
				//	//Debug.LogError(sheet.Information.Name + " " + (sheet.Characteristics.Alignment as AlignmentCharacteristic).Aligment);

				//	if(condition == null || condition.CheckOnce(node.FinalActor.Transform, blackboard))
				//	{
				//		return true;
				//	}
				//}
			}

			return false;
		}

#if UNITY_EDITOR
		protected override void OnTaskInspectorGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginVertical();
			base.OnTaskInspectorGUI();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
#endif
	}
}