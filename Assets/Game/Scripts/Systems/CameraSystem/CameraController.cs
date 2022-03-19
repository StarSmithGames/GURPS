using Cinemachine;

using DG.Tweening;

using UnityEngine;
using System.Linq;

using Zenject;

public class CameraController : IInitializable, ITickable
{
	public float movementSpeed = 10f;

	public float rotationSpeed = 1f;

	public Vector2 zoomMinMax = new Vector2(0.01f, 10f);
	public float zoomStandart = 5f;
	public float zoomSpeed = 10f;

	private float currentYRotation;
	private float currentZoom;

	private Vector3 cameraPivotPosition;
	private float cameraPivotYRotation;

	private CinemachineFramingTransposer transposer;

	private CinemachineBrain brain;
	private InputManager inputManager;

    public CameraController(CinemachineBrain brain, InputManager inputManager)
	{
		this.brain = brain;
		this.inputManager = inputManager;
	}

	public void Initialize()
	{
		transposer = (brain.ActiveVirtualCamera as CinemachineVirtualCamera).GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFramingTransposer;
		cameraPivotPosition = transposer.FollowTarget.localPosition;
		cameraPivotYRotation = transposer.FollowTarget.rotation.eulerAngles.y;

		SetZoom(zoomStandart);
	}
	public void Tick()
	{
		if (inputManager.GetKeyDown(KeyAction.CameraCenter))
		{
			CameraHome();
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

	public void SetFollowTarget(Transform target)
	{
		brain.ActiveVirtualCamera.Follow = target;
		brain.ActiveVirtualCamera.LookAt = target;
	}

	private void CameraHome()
	{
		transposer.FollowTarget.localPosition = cameraPivotPosition;
		transposer.FollowTarget.rotation = Quaternion.Euler(0, cameraPivotYRotation, 0);
	}

	private void SetZoom(float zoom)
	{
		currentZoom = Mathf.Clamp(zoom, zoomMinMax.x, zoomMinMax.y);

		transposer.m_CameraDistance = currentZoom;
	}
}