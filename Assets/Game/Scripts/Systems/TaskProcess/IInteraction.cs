using Game.Entities.Models;
using Game.Systems.InteractionSystem;
using Game.Systems.InventorySystem;

namespace Game.Systems
{
	public interface IInteraction
	{
		void Execute(IInteractable interactor);
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
						.Append(new GoToTaskAction(entity, container.InteractionPoint.GetIteractionPosition(entity)))
						.Append(() => container.Open(interactor));
					entity.TaskSequence.Execute();
				}
			}
		}
	}

	//public class TalkInteraction : IInteraction
	//{
	//	private IActor initiator;

	//	public TalkInteraction(IActor actor)
	//	{
	//		this.initiator = actor;
	//	}

	//	public void Execute(IInteractable interactor)
	//	{
	//		Talker.ABTalk(initiator, interactor as IActor);
	//	}
	//}
}