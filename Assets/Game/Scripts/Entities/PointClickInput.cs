using Cinemachine;

using Game.Managers.CharacterManager;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

public class PointClickInput : ITickable
{
	public bool IsMouseHolded { get; private set; }

	private CinemachineBrain camera;
	private InputManager inputManager;
	private CharacterManager characterManager;
	private Settings settings;

	[Inject]
	private void Construct(CinemachineBrain brain, InputManager inputManager, CharacterManager characterManager, GlobalSettings settings)
	{
		this.camera = brain;
		this.inputManager = inputManager;
		this.characterManager = characterManager;
		this.settings = settings.pointClickInput;
	}

	public void Tick()
	{
		IsMouseHolded = settings.isCanHoldMouse && inputManager.IsLeftMouseButtonPressed();

		if (!settings.isCanHoldMouse && inputManager.IsLeftMouseButtonDown() || IsMouseHolded && !EventSystem.current.IsPointerOverGameObject())
		{
			Ray mouseRay = camera.OutputCamera.ScreenPointToRay(inputManager.GetMousePosition());

			if (Physics.Raycast(mouseRay, out RaycastHit hit, settings.rayLength, settings.raycastLayerMask, QueryTriggerInteraction.Ignore))
			{
				characterManager.CurrentCharacter.SetTarget(hit.point);
			}
			else
			{
				characterManager.CurrentCharacter.SetTarget(Vector3.zero);
			}
		}
	}

	[System.Serializable]
	public class Settings
	{
		public bool isCanHoldMouse = true;
		public LayerMask raycastLayerMask = ~0;
		public float rayLength = 100f;
	}
}