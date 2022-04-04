
using EPOOutline;

using Game.Entities;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.Entities
{
	public abstract class Entity : InteractableModel, IEntity
	{
		public Transform Transform => transform;

		public virtual ISheet Sheet { get; private set; }

		public NavigationController Navigation { get; private set; }
		public CharacterController3D Controller { get; private set; }

		public Markers Markers { get; private set; }
		public Outlinable Outlines { get; private set; }

		public Transform CameraPivot { get; private set; }

		protected SignalBus signalBus;
		protected UIManager uiManager;

		[Inject]
		private void Construct(
			SignalBus signalBus,
			NavigationController navigationController,
			CharacterController3D controller,
			Markers markerController,
			Outlinable outline,
			[Inject(Id = "CameraPivot")] Transform cameraPivot,
			UIManager uiManager)
		{
			this.signalBus = signalBus;

			Navigation = navigationController;
			Controller = controller;
			Markers = markerController;
			Outlines = outline;
			CameraPivot = cameraPivot;
			this.uiManager = uiManager;

			Validate();
		}

		protected virtual void Start()
		{
			Outlines.enabled = false;

			ResetMarkers();
		}

		public void Freeze(bool trigger)
		{
			if (trigger)
			{
				Controller.Freeze();
			}
			else
			{
				Controller.UnFreeze();
			}
		}

		#region Observe
		public override void StartObserve()
		{
			base.StartObserve();
			uiManager.Battle.SetSheet(Sheet);
		}
		public override void EndObserve()
		{
			base.EndObserve();
			uiManager.Battle.SetSheet(null);
		}
		#endregion

		protected virtual void ResetMarkers()
		{
			Markers.FollowMarker.Enable(false);

			Markers.TargetMarker.transform.parent = null;
			Markers.TargetMarker.Enable(false);

			Markers.AreaMarker.Enable(false);

			Markers.LineMarker.Enable(false);
		}

		private void Validate()
		{
			Assert.IsNotNull(Navigation, $"Entity {gameObject.name} lost component.");
			Assert.IsNotNull(Controller, $"Entity {gameObject.name} lost component.");
			Assert.IsNotNull(Markers, $"Entity {gameObject.name} lost component.");
			Assert.IsNotNull(Outlines, $"Entity {gameObject.name} lost component.");
			Assert.IsNotNull(CameraPivot, $"Entity {gameObject.name} lost component.");
		}
	}
}