using System.Collections;
using System.Collections.Generic;
using System.Timers;

using UnityEngine;
using UnityEngine.Events;

namespace Game.Systems.InteractionSystem
{
	public class TaskSequence
	{
		public bool IsCanBeBreaked => IsSequenceProcess && ProcessTimeInMilliSeconds >= 250;//1/4 sec

		public bool IsSequenceProcess => sequenceCoroutine != null;
		private Coroutine sequenceCoroutine = null;

		private int ProcessTimeInMilliSeconds = -1;
		private Timer timer;

		private object currentTask = null;
		private List<object> tasks = new List<object>();


		private MonoBehaviour owner;

		public TaskSequence(MonoBehaviour owner)
		{
			this.owner = owner;
		}

		public TaskSequence Append(ITaskAction task)
		{
			tasks.Add(task);
			return this;
		}

		public TaskSequence Append(UnityAction action)
		{
			tasks.Add(action);
			return this;
		}

		private void Dispose()
		{
			tasks.Clear();
			sequenceCoroutine = null;
			currentTask = null;

			timer.Stop();
			ProcessTimeInMilliSeconds = -1;
		}

		public void Execute()
		{
			if (!IsSequenceProcess)
			{
				if (tasks.Count > 0)
				{
					sequenceCoroutine = owner.StartCoroutine(Sequence());
				}
			}
		}

		public void Execute(ITaskAction task)
		{
			if (!IsSequenceProcess)
			{
				if (task != null)
				{
					Append(task).Execute();
				}
			}
		}


		private IEnumerator Sequence()
		{
			StartTimer();

			for (int i = 0; i < tasks.Count; i++)
			{
				currentTask = tasks[i];

				if (currentTask is ITaskAction taskAction)
				{
					yield return taskAction.Implementation();

					if (taskAction.Status == TaskActionStatus.Cancelled)
					{
						Debug.LogError("taskAction.Status == TaskActionStatus.Cancelled, Breaked");
						break;
					}
				}
				else if (currentTask is UnityAction action)
				{
					action?.Invoke();
				}
			}

			Dispose();
		}

		private void StartTimer()
		{
			timer = new Timer();
			timer.Elapsed += new ElapsedEventHandler((o, e) => ProcessTimeInMilliSeconds = e.SignalTime.Millisecond);
			timer.Interval = 100;
			timer.Enabled = true;
		}
	}
}