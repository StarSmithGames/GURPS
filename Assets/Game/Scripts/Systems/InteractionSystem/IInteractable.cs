using Game.Entities.Models;
using Game.Systems.InventorySystem;

using UnityEngine;
using UnityEngine.Events;

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

	public class GoToPointInteraction : IInteraction
	{
		private InteractionPoint point;
		private UnityAction action;

		public GoToPointInteraction(InteractionPoint point, UnityAction action)
		{
			this.point = point;
			this.action = action;
		}

		public void Execute(IInteractable interactor)
		{
			if (point.IsInRange(interactor.Transform.position))
			{
				action?.Invoke();
			}
			else
			{
				if (interactor is IEntityModel entity)
				{
					entity.TaskSequence
						.Append(new GoToAction(entity, point.GetIteractionPosition(entity)))
						.Append(() => action?.Invoke());
					entity.TaskSequence.Execute();
				}
			}
		}

		public void Release() { }
	}

	public class ContainerInteraction : IInteraction
	{
		private IContainer container;

		public ContainerInteraction(IContainer container)
		{
			this.container = container;
		}

		public void Execute(IInteractable interactor)
		{
			if (container.InteractionPoint.IsInRange(interactor.Transform.position))
			{
				container.Open(interactor);
			}
			else
			{
				if (interactor is IEntityModel entity)
				{
					entity.TaskSequence
						.Append(new GoToAction(entity, container.InteractionPoint.GetIteractionPosition(entity)))
						.Append(() => container.Open(interactor));
					entity.TaskSequence.Execute();
				}
			}
		}

		public void Release()
		{
		}
	}
}