using Game.Entities;
using Game.Map;

using UnityEngine;

namespace Game.Systems.InteractionSystem
{
	public interface IInteractable
	{
		bool IsInteractable { get; }
		IInteraction Interaction { get; }
		Transform Transform { get; }

		bool InteractWith(IInteractable interactable);
	}

	public interface IInteraction
	{
		void Execute(IInteractable interactor);
		void Release();
	}

	public class BaseInteraction : IInteraction
	{
		private IInteractable self;

		public BaseInteraction(IInteractable interactable)
		{
			self = interactable;
		}

		public void Execute(IInteractable interactor)
		{

		}

		public void Release()
		{
			//not need
		}
	}

	public class WayPointInteraction : IInteraction
	{
		private IWayPoint wayPoint;

		public WayPointInteraction(IWayPoint wayPoint)
		{
			this.wayPoint = wayPoint;
		}

		public void Execute(IInteractable interactor)
		{
			if (wayPoint.InteractionPoint.IsInRange(interactor.Transform.position))
			{
				wayPoint.Action();
			}
			else
			{
				if(interactor is IEntity entity)
				{
					entity.TaskSequence
						.Append(new GoToAction(entity, wayPoint.InteractionPoint.GetIteractionPosition(entity)))
						.Append(wayPoint.Action);
					entity.TaskSequence.Execute();
				}
			}
		}

		public void Release()
		{
		}
	}
}