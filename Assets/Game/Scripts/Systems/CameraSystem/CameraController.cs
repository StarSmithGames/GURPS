using Cinemachine;
using DG.Tweening;
using UnityEngine;

using Zenject;
using Game.Managers.CharacterManager;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using Game.Managers.InputManager;
using Game.Entities;
using Game.Systems.DialogueSystem;

namespace Game.Systems.CameraSystem
{
	public class CameraController : IInitializable, ITickable, IDisposable
	{
		public float movementSpeed = 10f;

		public float rotationSpeed = 50f;

		public Vector2 zoomMinMax = new Vector2(0.01f, 10f);
		public float zoomStandart = 5f;
		public float zoomSpeed = 10f;
		private float currentZoom;

		private CameraPivot currentPivot;

		private bool IsInBlendTransition => tacticCoroutine != null;
		private Coroutine tacticCoroutine = null;
		private bool isTactic = false;

		private CinemachineFramingTransposer currentTransposer;

		private SignalBus signalBus;
		private CinemachineBrain brain;
		private List<CinemachineVirtualCamera> characterCamers;
		private InputManager inputManager;
		private CameraVision cameraVision;
		private AsyncManager asyncManager;
		private CharacterManager characterManager;
		private Character leader;

		public CameraController(SignalBus signalBus,
			CinemachineBrain brain,
			[Inject(Id = "CharacterCamers")] List<CinemachineVirtualCamera> characterCamers,
			InputManager inputManager,
			CameraVision cameraVision,
			AsyncManager asyncManager,
			CharacterManager characterManager)
		{
			this.signalBus = signalBus;
			this.brain = brain;
			this.characterCamers = characterCamers;
			this.inputManager = inputManager;
			this.cameraVision = cameraVision;
			this.asyncManager = asyncManager;
			this.characterManager = characterManager;
		}

		public void Initialize()
		{
			leader = characterManager.CurrentParty.LeaderParty;

			currentTransposer = (brain.ActiveVirtualCamera as CinemachineVirtualCamera).GetCinemachineComponent<CinemachineFramingTransposer>();

			SetZoom(zoomStandart);

			LookAt(characterManager.CurrentParty.LeaderParty);
			signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);

			signalBus?.Subscribe<StartDialogueSignal>(OnStartDialogue);
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
				if (!IsInBlendTransition)
				{
					isTactic = !isTactic;
					characterCamers.OrderByDescending((x) => x.Priority).First().gameObject.SetActive(!isTactic);
					tacticCoroutine = asyncManager.StartCoroutine(WaitWhileCamerasBlendes());
				}
			}

			#region Move
			if (inputManager.GetKey(KeyAction.CameraForward))
			{
				currentTransposer.FollowTarget.position += currentTransposer.FollowTarget.forward * movementSpeed * Time.deltaTime;
			}
			if (inputManager.GetKey(KeyAction.CameraBackward))
			{
				currentTransposer.FollowTarget.position += -currentTransposer.FollowTarget.forward * movementSpeed * Time.deltaTime;
			}
			if (inputManager.GetKey(KeyAction.CameraLeft))
			{
				currentTransposer.FollowTarget.position += -currentTransposer.FollowTarget.right * movementSpeed * Time.deltaTime;
			}
			if (inputManager.GetKey(KeyAction.CameraRight))
			{
				currentTransposer.FollowTarget.position += currentTransposer.FollowTarget.right * movementSpeed * Time.deltaTime;
			}
			#endregion

			#region Rotate
			if (inputManager.GetKey(KeyAction.CameraRotate))
			{
				currentTransposer.FollowTarget.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotationSpeed * 2 * Time.deltaTime, Space.World);
			}
			if (inputManager.GetKey(KeyAction.CameraRotateLeft))
			{
				currentTransposer.FollowTarget.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
			}
			if (inputManager.GetKey(KeyAction.CameraRotateRight))
			{
				currentTransposer.FollowTarget.Rotate(Vector3.down * rotationSpeed * Time.deltaTime, Space.World);
			}
			#endregion

			#region Zoom
			if (!cameraVision.IsUI)
			{
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
			}
			#endregion
		}

		public void CameraToHome()
		{
			DOTween.To(
				() => currentTransposer.FollowTarget.localPosition,//from
				(x) => currentTransposer.FollowTarget.localPosition = x,//step
				currentPivot.settings.startPosition,//to
				0.5f);//t

			currentTransposer.FollowTarget.DORotate(currentPivot.settings.startRotation.eulerAngles, 0.3f);
		}

		public void SetFollowTarget(CameraPivot pivot)
		{
			currentPivot = pivot;

			characterCamers.ForEach((x) =>
			{
				x.Follow = currentPivot.transform;
				x.LookAt = currentPivot.transform;
			});
		}

		public void LookAt(IEntity entity)
		{
			if (entity is Character character)
			{
				characterManager.CurrentParty.SetLeader(character);
			}

			SetFollowTarget(entity.CameraPivot);
			CameraToHome();
		}


		private void SetZoom(float zoom)
		{
			currentZoom = Mathf.Clamp(zoom, zoomMinMax.x, zoomMinMax.y);

			currentTransposer.m_CameraDistance = currentZoom;
		}

		private IEnumerator WaitWhileCamerasBlendes()
		{
			yield return null;
			yield return new WaitWhile(() => !brain.ActiveBlend?.IsComplete ?? false);

			currentTransposer = (brain.ActiveVirtualCamera as CinemachineVirtualCamera).GetCinemachineComponent<CinemachineFramingTransposer>();

			tacticCoroutine = null;
		}


		private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
		{
			leader = signal.leader;
			LookAt(leader);
		}

		private void OnStartDialogue(StartDialogueSignal signal)
		{
			SetZoom(zoomStandart);
		}
	}
}