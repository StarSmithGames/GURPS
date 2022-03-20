using Cinemachine;

using Game.Managers.CharacterManager;

using UnityEngine;
using UnityEngine.EventSystems;

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
	private CharacterManager characterManager;
	private Settings settings;

	public CameraVision(CinemachineBrain brain, InputManager inputManager, CharacterManager characterManager, GlobalSettings settings)
	{
		this.brain = brain;
		this.inputManager = inputManager;
		this.characterManager = characterManager;
		this.settings = settings.cameraVision;
	}

	public void Initialize()
	{

	}

	public void Tick()
	{
		RaycastHit hit;
		Ray mouseRay = brain.OutputCamera.ScreenPointToRay(inputManager.GetMousePosition());

		//Looking
		if (Physics.Raycast(mouseRay, out hit, settings.raycastLength, settings.raycastLayerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject())
		{
			CurrentEntity = hit.transform.root.GetComponent<IObservable>();
		}
		else
		{
			CurrentEntity = null;
		}

		//MouseHolding
		if (inputManager.IsLeftMouseButtonPressed())
		{
			if(CurrentEntity != null)
			{
				if (inputManager.IsLeftMouseButtonDown())
				{
					characterManager.CurrentCharacter.InteractWith(CurrentEntity);
				}
			}
			else
			{
				//Targeting
				if (settings.isCanHoldMouse || inputManager.IsLeftMouseButtonDown())
				{
					if (Physics.Raycast(mouseRay, out hit, settings.raycastLength, settings.raycastLayerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject())
					{
						characterManager.CurrentCharacter.Controller.SetDestination(hit.point);
					}
				}
			}
		}
	}
	
	[System.Serializable]
	public class Settings
	{
		public bool isCanHoldMouse = true;

		public LayerMask raycastLayerMask = ~0;
		public float raycastLength = 100f;
	}
}