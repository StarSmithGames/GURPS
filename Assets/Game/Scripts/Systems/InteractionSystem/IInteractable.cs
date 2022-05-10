using Game.Entities;

using UnityEngine;

namespace Game.Systems.InteractionSystem
{
	public interface IInteractable
	{
		bool IsInteractable { get; }

		bool IsInRange(IEntity entity);

		Vector3 GetIteractionPosition(IEntity entity);
	}
}