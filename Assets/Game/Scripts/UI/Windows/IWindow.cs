using UnityEngine.Events;

public interface IWindow
{
	bool IsShowing { get; }

	void Show(UnityAction callback = null);
	void Hide(UnityAction callback = null);
}