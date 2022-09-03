using UnityEngine;
using UnityEngine.Events;

namespace Game.Entities
{
	public interface IPathfinderable : ITransform
	{
		event UnityAction onTargetChanged;
		event UnityAction onDestinationChanged;

		bool IsHasTarget { get; }

		NavigationController Navigation { get; }
		IController Controller { get; }

		bool IsInReach(Vector3 point);

		void SetTarget(Vector3 point, float maxPathDistance = -1);
		void SetDestination(Vector3 destination, float maxPathDistance = -1);
		void Freeze(bool trigger);
		void Stop();
	}
}