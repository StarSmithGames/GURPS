namespace Game.Systems.InteractionSystem
{
	public interface IInteractable : ITransform
	{
		bool IsInteractable { get; }
		IInteraction Interaction { get; }

		InteractionPoint InteractionPoint { get; }

		bool InteractWith(IInteractable interactable);
		bool ExecuteInteraction(IInteraction interaction);
	}
}