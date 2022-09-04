using UnityEngine;
using UnityEngine.Events;

namespace Game.Systems.NavigationSystem
{
	public interface IController
	{
		event UnityAction onReachedDestination;

		bool IsHasTarget { get; }
		bool IsGrounded { get; }

		void Freeze(bool trigger);
		void Enable(bool trigger);

		bool SetDestination(Vector3 destination, float maxPathDistance = -1);
		void Stop();

		Vector3 GetVelocity();
	}
}