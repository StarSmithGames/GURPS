using UnityEngine;
using UnityEngine.Events;

namespace Game.Entities
{
	public interface IPathfinderable
	{
		event UnityAction onTargetChanged;
		event UnityAction onDestinationChanged;

		bool IsHasTarget { get; }

		Transform Transform { get; }

		NavigationController Navigation { get; }
		CharacterController3D Controller { get; }

		void SetTarget(Vector3 point, float maxPathDistance = -1);
		void SetDestination(Vector3 destination, float maxPathDistance = -1);
	}
}