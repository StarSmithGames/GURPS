using Cinemachine;

using UnityEngine;

using Zenject;

public class PointClickInput : MonoBehaviour
{
	public bool IsMouseHolded { get; private set; }

	[SerializeField] private bool isCanHoldMouse = true;
	[SerializeField] private LayerMask raycastLayerMask = ~0;

	private CinemachineBrain camera;
	private CharacterThirdPersonController controller;

	[Inject]
	private void Construct(CinemachineBrain brain, CharacterThirdPersonController controller)
	{
		this.camera = brain;
		this.controller = controller;
	}

	private void Update()
	{
		IsMouseHolded = isCanHoldMouse && Mouse.IsMouseButtonPressed();

		if (!isCanHoldMouse && Mouse.IsMouseButtonDown() || IsMouseHolded)
		{
			Ray mouseRay = camera.OutputCamera.ScreenPointToRay(Mouse.GetMousePosition());

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

public static class Mouse
{
	public static Vector2 GetMousePosition()
	{
		return Input.mousePosition;
	}

	public static bool IsMouseButtonDown()
	{
		return Input.GetMouseButtonDown(0);
	}
	public static bool IsMouseButtonPressed()
	{
		return Input.GetMouseButton(0);
	}
}