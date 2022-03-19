using Cinemachine;

using UnityEngine;

using Zenject;

public class PointClickInput : MonoBehaviour
{
	public bool IsMouseHolded { get; private set; }

	[SerializeField] private bool isCanHoldMouse = true;
	[SerializeField] private LayerMask raycastLayerMask = ~0;

	private CinemachineBrain camera;
	private InputManager inputManager;
	private CharacterThirdPersonController controller;

	[Inject]
	private void Construct(CinemachineBrain brain, InputManager inputManager, CharacterThirdPersonController controller)
	{
		this.camera = brain;
		this.inputManager = inputManager;
		this.controller = controller;
	}

	private void Update()
	{
		IsMouseHolded = isCanHoldMouse && inputManager.IsMouseButtonPressed();

		if (!isCanHoldMouse && inputManager.IsMouseButtonDown() || IsMouseHolded)
		{
			Ray mouseRay = camera.OutputCamera.ScreenPointToRay(inputManager.GetMousePosition());

			if (Physics.Raycast(mouseRay, out RaycastHit hit, 100f, raycastLayerMask, QueryTriggerInteraction.Ignore))
			{
				controller.SetDestination(hit.point);
			}
			else
			{
				controller.SetDestination(Vector3.zero);
			}
		}
	}
}