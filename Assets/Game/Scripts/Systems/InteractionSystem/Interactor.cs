namespace Game.Systems.InteractionSystem
{
	public static class Interactor
	{
		public static void ABInteraction(IInteractable initiator, IInteractable interactable)
		{
			initiator.InteractWith(interactable);
		}
	}
}