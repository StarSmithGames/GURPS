using Cinemachine;

using DG.Tweening;

using UnityEngine;

using Zenject;

using static CMF.SmoothPosition;

public class CameraController : MonoBehaviour
{
	[SerializeField] private CinemachineVirtualCamera camera;

	private InputManager inputManager;

	public float movementSpeed = 10f;

	public float rotationSpeed = 1f;

	public Vector2 zoomMinMax = new Vector2(0.01f, 10f);
	public float zoomStandart = 5f;
	public float zoomSpeed = 10f;

	private float currentYRotation;
	private float currentZoom;

	private CinemachineFramingTransposer transposer;
	private Vector3 cameraPivotPosition;
	private float cameraPivotYRotation;

	[Inject]
    private void Construct(InputManager inputManager)
	{
		this.inputManager = inputManager;
	}

	private void Start()
	{
		transposer = camera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFramingTransposer;

		cameraPivotPosition = transposer.FollowTarget.localPosition;
		cameraPivotYRotation = transposer.FollowTarget.rotation.eulerAngles.y;

		SetZoom(zoomStandart);
	}

	private void Update()
	{
		if (inputManager.GetKeyDown(KeyAction.CameraCenter))
		{
			transposer.FollowTarget.localPosition = cameraPivotPosition;
			transposer.FollowTarget.rotation = Quaternion.Euler(0, cameraPivotYRotation, 0);
		}

		if (inputManager.GetKey(KeyAction.CameraForward))
		{
			transposer.FollowTarget.position += transposer.FollowTarget.forward * movementSpeed * Time.deltaTime;
		}
		if (inputManager.GetKey(KeyAction.CameraBackward))
		{
			transposer.FollowTarget.position += -transposer.FollowTarget.forward * movementSpeed * Time.deltaTime;
		}
		if (inputManager.GetKey(KeyAction.CameraLeft))
		{
			transposer.FollowTarget.position += -transposer.FollowTarget.right * movementSpeed * Time.deltaTime;
		}
		if (inputManager.GetKey(KeyAction.CameraRight))
		{
			transposer.FollowTarget.position += transposer.FollowTarget.right * movementSpeed * Time.deltaTime;
		}


		if (inputManager.GetKey(KeyAction.CameraRotate))
		{
			Debug.LogError(inputManager.GetMousePosition());
		}
		if (inputManager.GetKey(KeyAction.CameraRotateLeft))
		{
			currentYRotation += rotationSpeed * Time.deltaTime;

			transposer.FollowTarget.Rotate(Vector3.up * currentYRotation, Space.World);

			currentYRotation = 0;
		}
		if (inputManager.GetKey(KeyAction.CameraRotateRight))
		{
			currentYRotation += rotationSpeed * Time.deltaTime;

			transposer.FollowTarget.Rotate(Vector3.down * currentYRotation, Space.World);

			currentYRotation = 0;
		}

		#region Zoom
		if (inputManager.GetKey(KeyAction.CameraZoomIn))
		{
			currentZoom += zoomSpeed * Time.deltaTime;

			SetZoom(currentZoom);
		}
		if (inputManager.IsScroolWheelDown())
		{
			currentZoom += zoomSpeed * 10f * Time.deltaTime;

			SetZoom(currentZoom);
		}

		if (inputManager.GetKey(KeyAction.CameraZoomOut))
		{
			currentZoom -= zoomSpeed * Time.deltaTime;

			SetZoom(currentZoom);
		}
		if (inputManager.IsScroolWheelUp())
		{
			currentZoom -= zoomSpeed * 10f * Time.deltaTime;

			SetZoom(currentZoom);
		}
		#endregion
	}

	private void SetZoom(float zoom)
	{
		currentZoom = Mathf.Clamp(zoom, zoomMinMax.x, zoomMinMax.y);

		transposer.m_CameraDistance = currentZoom;
	}
}