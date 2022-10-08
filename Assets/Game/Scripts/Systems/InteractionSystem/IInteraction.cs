using Game.Entities.Models;
using System;

namespace Game.Systems.InteractionSystem
{
	public interface IInteraction
	{
		void Execute(IInteractable interactor);
	}

	public class BaseInteraction : IInteraction
	{
		private InteractionPoint point;
		private Action<IInteractable> callback;

		public BaseInteraction(InteractionPoint point, Action<IInteractable> callback = null)
		{
			this.point = point;
			this.callback = callback;
		}

		public void Execute(IInteractable interactor)
		{
			if (point.IsInRange(interactor.Transform.position))
			{
				callback?.Invoke(interactor);
			}
			else
			{
				if (interactor is IEntityModel entity)
				{
					entity.TaskSequence
						.Append(new GoToTaskAction(entity, point.GetIteractionPosition(entity)))
						.Append(() => callback?.Invoke(interactor));
					entity.TaskSequence.Execute();
				}
			}
		}
	}
}