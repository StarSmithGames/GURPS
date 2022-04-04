using Game.Entities;

using UnityEngine;

namespace Game.Systems.InteractionSystem
{
	public interface IInteractable
	{
		bool IsInteractable { get; }

		Vector3 GetIteractionPosition(IEntity entity);

		void Interact();
		void InteractFrom(IEntity entity);
	}
}