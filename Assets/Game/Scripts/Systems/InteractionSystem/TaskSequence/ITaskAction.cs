using Game.Entities.Models;
using Game.Systems.NavigationSystem;

using System.Collections;

using UnityEngine;

namespace Game.Systems.InteractionSystem
{
	public interface ITaskAction
	{
		TaskActionStatus Status { get; }
		IEnumerator Implementation();
	}

	public abstract class TaskAction : ITaskAction
	{
		public TaskActionStatus Status => status;
		protected TaskActionStatus status = TaskActionStatus.Preparing;

		public abstract IEnumerator Implementation();
	}

	public class GoToTaskAction : TaskAction
	{
		private IPathfinderable pathfinderable;
		private Vector3 destination;

		public GoToTaskAction(IPathfinderable pathfinderable, Vector3 destination)
		{
			this.pathfinderable = pathfinderable;
			this.destination = destination;
		}

		public override IEnumerator Implementation()
		{
			status = TaskActionStatus.Preparing;
			pathfinderable.SetDestination(destination);

			Vector3 lastDestination = pathfinderable.Navigation.CurrentNavMeshDestination;

			status = TaskActionStatus.Running;

			yield return new WaitWhile(() =>
			{
				if (lastDestination != pathfinderable.Navigation.CurrentNavMeshDestination)
				{
					status = TaskActionStatus.Cancelled;
					return false;
				}
				return !pathfinderable.Navigation.NavMeshAgent.IsReachedDestination();
			});

			if (status != TaskActionStatus.Cancelled)
			{
				status = TaskActionStatus.Done;
			}
		}
	}

	public class RotateToTaskAction : TaskAction
	{
		private IEntityModel entity;
		private Vector3 point;
		private float duration;

		public RotateToTaskAction(IEntityModel entity, Vector3 point, float duration = 0.25f)
		{
			this.entity = entity;
			this.point = point;
			this.duration = duration;
		}
		public RotateToTaskAction(IEntityModel entity, Transform lookAt, float duration = 0.25f)
		{
			this.entity = entity;
			point = lookAt.position;
			this.duration = duration;
		}

		public override IEnumerator Implementation()
		{
			status = TaskActionStatus.Running;

			yield return (entity.Controller as CharacterController3D).RotateAnimatedTo(point, duration);
			yield return null;
			status = TaskActionStatus.Done;
		}
	}

	public enum TaskActionStatus
	{
		Preparing,
		Running,
		Cancelled,
		Done,
	}
}