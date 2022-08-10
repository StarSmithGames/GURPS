using UnityEngine.Events;

namespace Game.Managers.TransitionManager
{
	public interface ITransition
	{
		bool IsInProgress { get; }

		void In(UnityAction callback = null);
		void Out(UnityAction callback = null);

		void Terminate();
	}
}