using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace Game.Systems.QuestSystem
{
	[CreateAssetMenu(menuName = "Game/Quests/Quest", fileName = "Quest")]
	public class QuestData : SerializedScriptableObject
	{
		public string questName;
		[Space]
		public string title;

		[TextArea(5, 5)]
		public string description;
		[Space]
		[ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Title")]
		[NonSerialized, OdinSerialize]
		public List<QuestObjectiveData> objectives = new List<QuestObjectiveData>();
	}

	[System.Serializable]
	public class QuestObjectiveData
	{
		public string title;
		[TextArea(5, 5)]
		public string description;

		public bool isOverrideMainDescription = false;
		[ShowIf("isOverrideMainDescription")]
		[TextArea(5, 5)]
		public string mainDescription;

		[Space]
		[ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Title")]
		public List<QuestGoalData> goals = new List<QuestGoalData>();

		private string Title => title;
	}

	[System.Serializable]
	public class QuestGoalData
	{
		public string title;

		public IQuestGoal questGoal;

		private string Title => title;
	}
}