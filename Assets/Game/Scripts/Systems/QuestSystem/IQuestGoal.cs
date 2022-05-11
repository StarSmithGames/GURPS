using Game.Entities;
using Game.Systems.InventorySystem;

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
	}

	public class QuestGoalWait : QuestGoalBase { }

	public class QuestGoalKill : QuestGoalBase
	{
		public List<Target> targets = new List<Target>();

		public class Target
		{
			public NPCData data;
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