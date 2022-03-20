public interface IWindow
{
	bool IsShowing { get; }

	void Show();
	void Hide();
}