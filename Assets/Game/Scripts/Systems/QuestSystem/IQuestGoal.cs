using Game.Entities;
using Game.Systems.InventorySystem;

using System;
using System.Collections.Generic;

namespace Game.Systems.QuestSystem
{
	public interface IQuestGoal
	{
		QuestGoalStatus CurrentStatus { get; }

		void SetStatus(QuestGoalStatus status);
	}

	public abstract class QuestGoalBase : IQuestGoal
	{
		public QuestGoalStatus CurrentStatus { get; protected set; }

		public virtual void SetStatus(QuestGoalStatus status)
		{
			CurrentStatus = status;
		}

		public static IQuestGoal GetGoalType(Type type)
		{
			if (type == typeof(QuestGoalWait))
			{
				return new QuestGoalWait();
			}
			else if (type == typeof(QuestGoalKill))
			{
				return new QuestGoalKill();
			}
			else if (type == typeof(QuestGoalDelivery))
			{
				return new QuestGoalDelivery();
			}
			else if(type == typeof(QuestGoalGathering))
			{
				return new QuestGoalGathering();
			}

			return null;
		}
	}

	public class QuestGoalWait : QuestGoalBase { }

	public class QuestGoalKill : QuestGoalBase
	{
		public List<Target> targets = new List<Target>();

		public class Target
		{
			public NonPlayableCharacterData data;
			public int count = 1;//min 1
		}
	}

	public class QuestGoalDelivery : QuestGoalBase
	{
		public List<Item> item = new List<Item>();
	}

	public class QuestGoalGathering : QuestGoalBase
	{

	}

	
	public enum QuestGoalStatus
	{
		Unassigned,
		Success,
		Failure,
	}
}