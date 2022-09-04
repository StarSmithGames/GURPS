using Cinemachine;

using Game.Managers.InputManager;

using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

namespace Game.Systems.CameraSystem
{
	public class CameraVision : IInitializable, IDisposable, ITickable, ICameraVision
	{
		public bool IsCanHoldMouse { get; protected set; }
		public bool IsMouseHit { get; protected set; }
		public bool IsUI { get; protected set; }
		public RaycastHit Hit { get; protected set; }
		public Vector3 HitPoint { get; protected set; }

		protected IObservable CurrentObserve
		{
			get => currentEntity;
			set
			{
				if (currentEntity != value)
				{
					currentEntity?.EndObserve();
					currentEntity = value;
					currentEntity?.StartObserve();

					OnHoverObserveChanged();
				}
				else
				{
					currentEntity?.Observe();
				}
			}
		}
		protected IObservable currentEntity = null;

		protected Settings settings;
		protected SignalBus signalBus;
		protected CinemachineBrain brain;
		protected InputManager inputManager;

		public CameraVision(SignalBus signalBus, CinemachineBrain brain, InputManager inputManager)
		{
			this.signalBus = signalBus;
			this.brain = brain;
			this.inputManager = inputManager;
			//this.settings = settings.cameraVisionMap;
		}


		public virtual void Initialize()
		{
			IsCanHoldMouse = settings.isCanHoldMouse;
		}

		public virtual void Dispose()
		{

		}

		public virtual void Tick()
		{
			IsUI = EventSystem.current.IsPointerOverGameObject();
			Ray mouseRay = brain.OutputCamera.ScreenPointToRay(inputManager.GetMousePosition());
			IsMouseHit = Physics.Raycast(mouseRay, out RaycastHit hit, settings.raycastLength, settings.raycastLayerMask);
			IsUI = EventSystem.current.IsPointerOverGameObject();
			Hit = hit;
			HitPoint = hit.point;

			//Looking
			CurrentObserve = IsMouseHit && !IsUI ? Hit.transform.root.GetComponent<IObservable>() : null;

			HandleHover(HitPoint);
			HandleMouseClick(HitPoint);
		}


		protected virtual void HandleHover(Vector3 point) { }

		protected virtual void HandleMouseClick(Vector3 point) { }

		protected virtual void OnHoverObserveChanged() { }



		[System.Serializable]
		public class Settings
		{
			public bool isCanHoldMouse = true;

			public LayerMask raycastLayerMask = ~0;
			public float raycastLength = 100f;
		}
	}
}