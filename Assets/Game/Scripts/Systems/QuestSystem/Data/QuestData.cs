using Sirenix.OdinInspector;

using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using System;
using ParadoxNotion.Design;
using WebSocketSharp;
using ParadoxNotion;
using Game.Systems.DialogueSystem.Nodes;
using System.Reflection;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Systems.QuestSystem
{
	[CreateAssetMenu(menuName = "Game/Quests/Quest", fileName = "Quest")]
	public class QuestData : ScriptableObject
	{
		public string Title => title.GetCurrent().value;

		public I2Texts<I2Text> title;
		public I2Texts<I2Text> description;
		public List<QuestObjectiveData> objectives = new List<QuestObjectiveData>();
	}

	[System.Serializable]
	public class QuestObjectiveData
	{
		public I2Texts<I2Text> title;
		public I2Texts<I2Text> description;

		public bool isOverrideMainDescription = false;
		public I2Texts<I2Text> mainDescription;

		public List<QuestGoalData> goals = new List<QuestGoalData>();

#if UNITY_EDITOR
		public bool isShowFoldout = false;

		public void OnGUI(UnityEngine.Object target)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			GUILayout.BeginVertical();
			title.OnGUI("Title");
			description.OnGUI("Description", true);
			isOverrideMainDescription = EditorGUILayout.Toggle("Override Main Description", isOverrideMainDescription);
			if (isOverrideMainDescription)
			{
				mainDescription.OnGUI("Main Description", true);
			}

			GUILayout.Label($"Goals Count {goals.Count}");
			if (GUILayout.Button("Add Goal"))
			{
				var goal = new QuestGoalData();
				goal.title = new I2Texts<I2Text>();
				goals.Add(goal);

				EditorUtility.SetDirty(target);
			}

			EditorUtils.ReorderableList(goals, (i, picked) =>
			{
				var goal = goals[i];

				GUILayout.BeginHorizontal("box");
				var text = string.Format("{0} {1} {2}", goal.isShowFoldout ? "-" : "+", $"[{i + 1}]", $"{goal.title.Text} {(goal.questGoal == null ? "NULL" : "")}");
				if (GUILayout.Button(text, (GUIStyle)"label", GUILayout.Width(0), GUILayout.ExpandWidth(true)))
				{
					goal.isShowFoldout = !goal.isShowFoldout;
				}

				if (GUILayout.Button("X", GUILayout.Width(20)))
				{
					goals.RemoveAt(i);

					EditorUtility.SetDirty(target);
				}
				GUILayout.EndHorizontal();

				if (goal.isShowFoldout)
				{
					goal.OnGUI(target);
				}
				else
				{
					goal.isShowFoldout = false;
					goal.title.isShowFoldout = false;
				}
			});
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
#endif
	}

	[System.Serializable]
	public class QuestGoalData
	{
		public I2Texts<I2Text> title;

		[SerializeField, SerializeReference] public IQuestGoal questGoal;

#if UNITY_EDITOR
		public bool isShowFoldout = false;

		public void OnGUI(UnityEngine.Object target)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			GUILayout.BeginVertical();
			title.OnGUI("Title");

			if(questGoal == null)
			{
				EditorGUILayout.HelpBox("Quest Goal == null", MessageType.Error);
			}
			else
			{
				GUILayout.Label($"Quest Goal == {questGoal.GetType()}");
			}

			if (GUILayout.Button("Select Goal"))
			{
				GenericMenu menu = new GenericMenu();

				var subclassTypes = Assembly
				   .GetAssembly(typeof(QuestGoalBase))
				   .GetTypes()
				   .Where(t => t.IsSubclassOf(typeof(QuestGoalBase))).ToList();

				for (int i = 0; i < subclassTypes.Count; i++)
				{
					menu.AddItem(new GUIContent(subclassTypes[i].ToString()), false, (obj) =>
					{
						questGoal = (QuestGoalBase)QuestGoalBase.GetGoalType(subclassTypes[(int)obj]);
						
						EditorUtility.SetDirty(target);
					}, i);
				}

				menu.ShowAsContext();
			}

			//questGoal = EditorGUILayout.ObjectField(questGoal, typeof(UnityEngine.Object), true);

			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
#endif
	}



#if UNITY_EDITOR
	[CustomEditor(typeof(QuestData))]
	public class QuestDataEditor : Editor
	{
		private QuestData QuestData => (QuestData)target;
		public override void OnInspectorGUI()
		{
			QuestData.title.OnGUI("Title");
			QuestData.description.OnGUI("Description", true);

			GUILayout.Label($"Objectives Count {QuestData.objectives.Count}");

			if (GUILayout.Button("Add Objective"))
			{
				var objective = new QuestObjectiveData();
				objective.title = new I2Texts<I2Text>();
				objective.description = new I2Texts<I2Text>();
				objective.mainDescription = new I2Texts<I2Text>();
				QuestData.objectives.Add(objective);

				EditorUtility.SetDirty(target);
			}
			if (QuestData.objectives.Count == 0)
			{
				return;
			}


			EditorUtils.ReorderableList(QuestData.objectives, (i, picked) =>
			{
				var objective = QuestData.objectives[i];

				GUILayout.BeginHorizontal("box");
				var text = string.Format("{0} {1} {2}", objective.isShowFoldout ? "-" : "+", $"[{i + 1}]", objective.title.Text);
				if (GUILayout.Button(text, (GUIStyle)"label", GUILayout.Width(0), GUILayout.ExpandWidth(true)))
				{
					objective.isShowFoldout = !objective.isShowFoldout;
				}

				if (GUILayout.Button("X", GUILayout.Width(20)))
				{
					QuestData.objectives.RemoveAt(i);

					EditorUtility.SetDirty(target);
				}
				GUILayout.EndHorizontal();

				if (objective.isShowFoldout)
				{
					objective.OnGUI(target);
				}
				else
				{
					objective.isShowFoldout = false;
					objective.title.isShowFoldout = false;
					objective.description.isShowFoldout = false;
					objective.mainDescription.isShowFoldout = false;
				}
			});
		}
	}
#endif
}