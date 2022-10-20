public interface IActivation
{
	bool IsActive { get; }

	void Activate();
	void Deactivate();
}