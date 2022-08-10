using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.QuestSystem
{
	public class QuestManager
	{
		public List<Quest> AllQuests { get; private set; }
		public List<Quest> CurrentQuests { get; private set; }

		private NotificationSystem.NotificationSystem notificationSystem;

		public QuestManager(NotificationSystem.NotificationSystem notificationSystem)
		{
			this.notificationSystem = notificationSystem;

			 CurrentQuests = new List<Quest>();
		}


		public void AddQuest(Quest quest)
		{
			if (!CurrentQuests.Contains(quest))
			{
				CurrentQuests.Add(quest);

				notificationSystem.PushJournal(quest.CurrentData.Title);
			}
		}


		public void SetQuestStatus(string questName, QuestStatus status)
		{

		}


		private void LoadData(Data data)
		{
			CurrentQuests.Clear();

			for (int i = 0; i < data.currentQuests.Count; i++)
			{
				CurrentQuests.Add(new Quest(data.currentQuests[i]));
			}
		}


		public Data GetData()
		{
			List<Quest.Data> currentQuests = new List<Quest.Data>();

			for (int i = 0; i < CurrentQuests.Count; i++)
			{
				currentQuests.Add(CurrentQuests[i].GetData());
			}

			return new Data()
			{
				currentQuests = currentQuests,
			};
		}

		public class Data
		{
			public List<Quest.Data> currentQuests;
		}
	}
}