using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game.Systems.QuestSystem
{
	[System.Serializable]
	public class Quest
	{
		public QuestData CurrentData => currentData;
		[SerializeField] private QuestData currentData;

		public QuestStatus CurrentStatus => status;
		[SerializeField] private QuestStatus status = QuestStatus.Unassigned;

		private List<QuestObjective> objectives;

		public Quest(Data data)
		{
			currentData = data.currentData;
			status = data.status;
			objectives = data.objectives;
		}

		public void Initialize()
		{
			objectives = new List<QuestObjective>();

			for (int i = 0; i < currentData.objectives.Count; i++)
			{
				var goals = currentData.objectives[i].goals.Select((x) => x.questGoal).ToList();
				objectives.Add(new QuestObjective() { goals = goals });
			}
		}

		public void SetStatus(QuestStatus status)
		{
			this.status = status;
		}

		public Data GetData()
		{
			return new Data()
			{
				currentData = currentData,
				status = status,
				objectives = objectives,
			};
		}

		public class Data
		{
			public QuestData currentData;
			public QuestStatus status;
			public List<QuestObjective> objectives;
		}
	}

	public class QuestObjective
	{
		public QuestStatus CurrentStatus { get; }

		public List<IQuestGoal> goals = new List<IQuestGoal>();
	}

	public enum QuestStatus
	{
		Unassigned,
		Active,
		Success,
		Failure,
	}
}