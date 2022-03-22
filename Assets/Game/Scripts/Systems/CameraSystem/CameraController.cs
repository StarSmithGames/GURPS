using Cinemachine;

using UnityEngine;

using Zenject;
using Game.Managers.CharacterManager;
using System;

public class CameraController : IInitializable, ITickable, IDisposable
{
	public float movementSpeed = 10f;

	public float rotationSpeed = 50f;

	public Vector2 zoomMinMax = new Vector2(0.01f, 10f);
	public float zoomStandart = 5f;
	public float zoomSpeed = 10f;

	private float currentZoom;

	private Vector3 cameraPivotPosition;
	private float cameraPivotYRotation;

	private CinemachineFramingTransposer transposer;

	private SignalBus signalBus;
	private CinemachineBrain brain;
	private InputManager inputManager;

    public CameraController(SignalBus signalBus, CinemachineBrain brain, InputManager inputManager)
	{
		this.signalBus = signalBus;
		this.brain = brain;
		this.inputManager = inputManager;
	}

	public void Initialize()
	{
		transposer = (brain.ActiveVirtualCamera as CinemachineVirtualCamera).GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFramingTransposer;
		cameraPivotPosition = transposer.FollowTarget.localPosition;
		cameraPivotYRotation = transposer.FollowTarget.rotation.eulerAngles.y;

		SetZoom(zoomStandart);

		signalBus?.Subscribe<SignalCharacterChanged>(OnCharacterChanged);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalCharacterChanged>(OnCharacterChanged);
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
			transposer.FollowTarget.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotationSpeed * 2 * Time.deltaTime, Space.World);

		}
		if (inputManager.GetKey(KeyAction.CameraRotateLeft))
		{
			transposer.FollowTarget.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
		}
		if (inputManager.GetKey(KeyAction.CameraRotateRight))
		{
			transposer.FollowTarget.Rotate(Vector3.down * rotationSpeed * Time.deltaTime, Space.World);
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


	private void OnCharacterChanged(SignalCharacterChanged signal)
	{
		SetFollowTarget(signal.character.CameraPivot);
	}
}