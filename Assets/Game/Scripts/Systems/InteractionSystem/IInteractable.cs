using UnityEngine;

namespace Game.Systems.InteractionSystem
{
	public interface IInteractable
	{
		bool IsInteractable { get; }

		Vector3 InteractPosition { get; }

		void Interact();
		void InteractFrom(IEntity entity);
	}
}