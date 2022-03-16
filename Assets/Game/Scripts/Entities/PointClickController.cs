using Cinemachine;

using UnityEngine;

using Zenject;

public class PointClickController : MonoBehaviour
{
	public bool IsMouseHolded { get; private set; }

	[SerializeField] private bool isCanHoldMouse = true;
	[SerializeField] private LayerMask raycastLayerMask = ~0;

	private CinemachineBrain camera;
	private Mover mover;

	[Inject]
	private void Construct(CinemachineBrain brain, Mover mover)
	{
		this.camera = brain;
		this.mover = mover;
	}

	private void Update()
	{
		IsMouseHolded = isCanHoldMouse && Mouse.IsMouseButtonPressed();

		if (!isCanHoldMouse && Mouse.IsMouseButtonDown() || IsMouseHolded)
		{
			Ray mouseRay = camera.OutputCamera.ScreenPointToRay(Mouse.GetMousePosition());

			if (Physics.Raycast(mouseRay, out RaycastHit hit, 100f, raycastLayerMask, QueryTriggerInteraction.Ignore))
			{
				mover.SetDestination(hit.point);
			}
			else
			{
				mover.SetDestination(Vector3.zero);
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