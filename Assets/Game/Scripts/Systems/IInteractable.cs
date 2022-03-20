public interface IInteractable
{
	bool IsInteractable { get; }

	void Interact();
	void InteractFrom(IEntity entity);
}