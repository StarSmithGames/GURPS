public interface IActivation
{
	bool IsActive { get; }

	void Activate();
	void Deactivate();
}

public interface IActivation<T>
{
	bool IsActive { get; }

	void Activate(T provider);
	void Deactivate();
}