using Cinemachine;

using UnityEngine;

using Zenject;

public class CameraVision : IInitializable, ITickable
{
	private IObservable CurrentEntity
	{
		get => currentEntity;
		set
		{
			if (currentEntity != value)
			{
				currentEntity?.EndObserve();
				currentEntity = value;
				currentEntity?.StartObserve();
			}
			else
			{
				currentEntity?.Observe();
			}
		}
	}
	private IObservable currentEntity = null;


	private CinemachineBrain brain;
	private InputManager inputManager;
	private Settings settings;

	public CameraVision(CinemachineBrain brain, InputManager inputManager, GlobalSettings settings)
	{
		this.brain = brain;
		this.inputManager = inputManager;
		this.settings = settings.cameraVision;
	}

	public void Initialize()
	{

	}

	public void Tick()
	{
		Ray mouseRay = brain.OutputCamera.ScreenPointToRay(inputManager.GetMousePosition());

		if (Physics.Raycast(mouseRay, out RaycastHit hit, settings.raycastLength, settings.raycastLayerMask, QueryTriggerInteraction.Ignore))
		{
			CurrentEntity = hit.transform.root.GetComponent<IObservable>();
		}
		else
		{
			CurrentEntity = null;
		}
	}


	[System.Serializable]
	public class Settings
	{
		public float raycastLength = 100f;
		public LayerMask raycastLayerMask = ~0;
	}
}