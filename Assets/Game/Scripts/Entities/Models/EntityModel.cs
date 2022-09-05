using Game.Systems.InteractionSystem;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using Zenject;
using System.Collections;
using Game.Systems.NavigationSystem;
using Game.Systems;

namespace Game.Entities.Models
{
	public interface IEntityModel : IPathfinderable, IObservable, IInteractable
	{
		MonoBehaviour MonoBehaviour { get; }
		TaskSequence TaskSequence { get; }
	}

	public abstract partial class EntityModel : Model, IEntityModel
	{
		public MonoBehaviour MonoBehaviour => this;
		public TaskSequence TaskSequence { get; private set; }

		protected SignalBus signalBus;

		[Inject]
		private void Construct(
			SignalBus signalBus,
			NavigationController navigationController,
			IController controller)
		{
			this.signalBus = signalBus;

			Navigation = navigationController;
			Controller = controller;

			TaskSequence = new TaskSequence(this);

			Validate();
		}

		protected virtual void OnDestroy() { }

		protected virtual IEnumerator Start()
		{
			yield return null;
		}

		private void Validate()
		{
			Assert.IsNotNull(Navigation, $"Entity {gameObject.name} lost component.");
			Assert.IsNotNull(Controller, $"Entity {gameObject.name} lost component.");
		}
	}

	//IPathfinderable implementation
	partial class EntityModel
	{
		public event UnityAction onTargetChanged;
		public event UnityAction onDestinationChanged;

		public bool IsHasTarget => Controller.IsHasTarget;

		public NavigationController Navigation { get; private set; }
		public IController Controller { get; private set; }

		public virtual bool IsInReach(Vector3 point)
		{
			return Navigation.IsTargetInReach(point);
		}

		public virtual void SetTarget(Vector3 point, float maxPathDistance = -1)
		{
			Navigation.SetTarget(point, maxPathDistance: maxPathDistance);
			onTargetChanged?.Invoke();
		}

		public virtual void SetDestination(Vector3 destination, float maxPathDistance = -1)
		{
			Controller.SetDestination(destination, maxPathDistance: maxPathDistance);
			onDestinationChanged?.Invoke();
		}

		public void Freeze(bool trigger)
		{
			Controller.Freeze(trigger);
		}

		public virtual void Stop()
		{
			if (IsHasTarget)
			{
				Controller.Stop();
			}
		}
	}
}