using Cinemachine;

using UnityEngine;

using Zenject;
using Game.Managers.CharacterManager;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
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

	private bool isTactic = false;

	private CinemachineFramingTransposer transposerMain;
	private CinemachineFramingTransposer transposerSpare;

	private SignalBus signalBus;
	private CinemachineBrain brain;
	private List<CinemachineVirtualCamera> characterCamers;
	private InputManager inputManager;
	private AsyncManager asyncManager;


	public CameraController(SignalBus signalBus,
		CinemachineBrain brain,
		[Inject(Id = "CharacterCamers")] List<CinemachineVirtualCamera> characterCamers,
		InputManager inputManager,
		AsyncManager asyncManager)
	{
		this.signalBus = signalBus;
		this.brain = brain;
		this.characterCamers = characterCamers;
		this.inputManager = inputManager;
		this.asyncManager = asyncManager;
	}

	public void Initialize()
	{
		transposerMain = (brain.ActiveVirtualCamera as CinemachineVirtualCamera).GetCinemachineComponent<CinemachineFramingTransposer>();
		transposerSpare = transposerMain;
		cameraPivotPosition = transposerMain.FollowTarget.localPosition;
		cameraPivotYRotation = transposerMain.FollowTarget.rotation.eulerAngles.y;

		SetZoom(zoomStandart);

		signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
	}

	public void Tick()
	{
		if (inputManager.GetKeyDown(KeyAction.CameraCenter))
		{
			CameraToHome();
		}

		if (inputManager.GetKeyDown(KeyAction.TacticalCamera))
		{
			isTactic = !isTactic;
			characterCamers.OrderByDescending((x) => x.Priority).First().gameObject.SetActive(!isTactic);
			asyncManager.StartCoroutine(WaitWhileCamerasBlendes());
		}

		#region Move
		if (inputManager.GetKey(KeyAction.CameraForward))
		{
			transposerSpare.FollowTarget.position += transposerMain.FollowTarget.forward * movementSpeed * Time.deltaTime;
		}
		if (inputManager.GetKey(KeyAction.CameraBackward))
		{
			transposerSpare.FollowTarget.position += -transposerMain.FollowTarget.forward * movementSpeed * Time.deltaTime;
		}
		if (inputManager.GetKey(KeyAction.CameraLeft))
		{
			transposerSpare.FollowTarget.position += -transposerMain.FollowTarget.right * movementSpeed * Time.deltaTime;
		}
		if (inputManager.GetKey(KeyAction.CameraRight))
		{
			transposerSpare.FollowTarget.position += transposerMain.FollowTarget.right * movementSpeed * Time.deltaTime;
		}
		#endregion

		#region Rotate
		if (inputManager.GetKey(KeyAction.CameraRotate))
		{
			transposerMain.FollowTarget.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotationSpeed * 2 * Time.deltaTime, Space.World);
		}
		if (inputManager.GetKey(KeyAction.CameraRotateLeft))
		{
			transposerMain.FollowTarget.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
		}
		if (inputManager.GetKey(KeyAction.CameraRotateRight))
		{
			transposerMain.FollowTarget.Rotate(Vector3.down * rotationSpeed * Time.deltaTime, Space.World);
		}
		#endregion

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

		CameraToHome();
	}

	public void CameraToHome()
	{
		transposerMain.FollowTarget.localPosition = cameraPivotPosition;
		transposerMain.FollowTarget.rotation = Quaternion.Euler(0, cameraPivotYRotation, 0);
	}

	private void SetZoom(float zoom)
	{
		currentZoom = Mathf.Clamp(zoom, zoomMinMax.x, zoomMinMax.y);

		transposerSpare.m_CameraDistance = currentZoom;
	}


	private IEnumerator WaitWhileCamerasBlendes()
	{
		yield return null;
		yield return new WaitWhile(() => !brain.ActiveBlend?.IsComplete ?? false);
		transposerSpare = (brain.ActiveVirtualCamera as CinemachineVirtualCamera).GetCinemachineComponent<CinemachineFramingTransposer>();
	}

	private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
	{
		SetFollowTarget(signal.leader.CameraPivot);
	}
}