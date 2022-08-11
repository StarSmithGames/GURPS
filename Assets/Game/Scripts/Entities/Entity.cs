using EPOOutline;
using Game.Systems.CameraSystem;
using Game.Systems.DamageSystem;
using Game.Systems.DialogueSystem;
using Game.Systems.FloatingTextSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using NodeCanvas.DialogueTrees;

using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using DG.Tweening;

using Zenject;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Game.Entities
{
	public abstract partial class Entity : InteractableModel, IEntity
	{
		public MonoBehaviour MonoBehaviour => this;
		public Transform Transform => transform;

		public virtual ISheet Sheet { get; private set; }

		public CameraPivot CameraPivot { get; private set; }

		protected SignalBus signalBus;

		[Inject]
		private void Construct(
			SignalBus signalBus,
			NavigationController navigationController,
			IController controller,
			CameraPivot cameraPivot)
		{
			this.signalBus = signalBus;

			Navigation = navigationController;
			Controller = controller;

			CameraPivot = cameraPivot;

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
			Assert.IsNotNull(CameraPivot, $"Entity {gameObject.name} lost component.");
		}
	}

	//IPathfinderable implementation
	partial class Entity
	{
		public event UnityAction onTargetChanged;
		public event UnityAction onDestinationChanged;

		public bool IsHasTarget => Controller.IsHasTarget;

		public NavigationController Navigation { get; private set; }
		public IController Controller { get; private set; }

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

	//IDamegeable, IKillable implementation
	partial class Entity
	{
		public event UnityAction<IEntity> onDied;

		public virtual void Kill()
		{
			Controller.Enable(false);
			onDied?.Invoke(this);
		}

		public virtual Damage GetDamage()
		{
			return null;
		}

		public virtual void ApplyDamage<T>(T value) { }

		protected Vector2 GetDamageFromTable()
		{
			return new Vector2(1, 7);
		}
	}
}