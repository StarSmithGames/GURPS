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
using Game.Managers.GameManager;
using Game.Entities.Models;
using Game.Managers.PartyManager;

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

		private CinemachineFramingTransposer CurrentTransposer
		{
			set
			{
				currentTransposer = value;
			}
			get
			{
				if(currentTransposer == null)
				{
					if(brain.ActiveVirtualCamera == null)
					{
						currentTransposer = camers.FirstOrDefault()?.GetCinemachineComponent<CinemachineFramingTransposer>();
					}
					else
					{
						currentTransposer = (brain.ActiveVirtualCamera as CinemachineVirtualCamera).GetCinemachineComponent<CinemachineFramingTransposer>();
					}
				}
				return currentTransposer;
			}
		}
		private CinemachineFramingTransposer currentTransposer;

		private SignalBus signalBus;
		private CinemachineBrain brain;
		private List<CinemachineVirtualCamera> camers;
		private InputManager inputManager;
		private ICameraVision cameraVision;
		private AsyncManager asyncManager;
		private CharacterManager characterManager;
		private GameManager gameManager;

		private CharacterModel leader;

		public CameraController(SignalBus signalBus,
			CinemachineBrain brain,
			[Inject(Id = "Camers")] List<CinemachineVirtualCamera> camers,
			InputManager inputManager,
			ICameraVision cameraVision,
			AsyncManager asyncManager,
			CharacterManager characterManager,
			GameManager gameManager)
		{
			this.signalBus = signalBus;
			this.brain = brain;
			this.camers = camers;
			this.inputManager = inputManager;
			this.cameraVision = cameraVision;
			this.asyncManager = asyncManager;
			this.characterManager = characterManager;
			this.gameManager = gameManager;
		}

		public void Initialize()
		{
			//leader = characterManager.CurrentParty.LeaderParty;

			SetZoom(zoomStandart);

			//LookAt(characterManager.CurrentParty.LeaderParty);
			signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);

			//signalBus?.Subscribe<StartDialogueSignal>(OnStartDialogue);
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
				if (!IsInBlendTransition && gameManager.CurrentGameLocation == GameLocation.Location)
				{
					isTactic = !isTactic;
					camers.OrderByDescending((x) => x.Priority).First().gameObject.SetActive(!isTactic);
					tacticCoroutine = asyncManager.StartCoroutine(WaitWhileCamerasBlendes());
				}
			}

			#region Move
			//if (!leader.IsInDialogue)
			{
				if (inputManager.GetKey(KeyAction.CameraForward))
				{
					CurrentTransposer.FollowTarget.position += CurrentTransposer.FollowTarget.forward * movementSpeed * Time.deltaTime;
				}
				if (inputManager.GetKey(KeyAction.CameraBackward))
				{
					CurrentTransposer.FollowTarget.position += -CurrentTransposer.FollowTarget.forward * movementSpeed * Time.deltaTime;
				}
				if (inputManager.GetKey(KeyAction.CameraLeft))
				{
					CurrentTransposer.FollowTarget.position += -CurrentTransposer.FollowTarget.right * movementSpeed * Time.deltaTime;
				}
				if (inputManager.GetKey(KeyAction.CameraRight))
				{
					CurrentTransposer.FollowTarget.position += CurrentTransposer.FollowTarget.right * movementSpeed * Time.deltaTime;
				}
			}
			#endregion

			#region Rotate
			if (inputManager.GetKey(KeyAction.CameraRotate))
			{
				CurrentTransposer.FollowTarget.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotationSpeed * 2 * Time.deltaTime, Space.World);
			}
			if (inputManager.GetKey(KeyAction.CameraRotateLeft))
			{
				CurrentTransposer.FollowTarget.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
			}
			if (inputManager.GetKey(KeyAction.CameraRotateRight))
			{
				CurrentTransposer.FollowTarget.Rotate(Vector3.down * rotationSpeed * Time.deltaTime, Space.World);
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
				() => CurrentTransposer.FollowTarget.localPosition,//from
				(x) => CurrentTransposer.FollowTarget.localPosition = x,//step
				currentPivot.settings.startPosition,//to
				0.5f);//t

			CurrentTransposer.FollowTarget.DORotate(currentPivot.settings.startRotation.eulerAngles, 0.3f);
		}

		public void SetFollowTarget(CameraPivot pivot)
		{
			currentPivot = pivot;

			camers.ForEach((x) =>
			{
				x.Follow = currentPivot.transform;
				x.LookAt = currentPivot.transform;
			});
		}

		public void LookAt(IEntityModel entity)
		{
			if (entity is CharacterModel character)
			{
				//characterManager.CurrentParty.SetLeader(character);
			}

			SetFollowTarget(entity.CameraPivot);
			CameraToHome();
		}


		private void SetZoom(float zoom)
		{
			currentZoom = Mathf.Clamp(zoom, zoomMinMax.x, zoomMinMax.y);

			CurrentTransposer.m_CameraDistance = currentZoom;
		}
		private void AnimateZoom(float zoom)
		{
			currentZoom = Mathf.Clamp(zoom, zoomMinMax.x, zoomMinMax.y);

			DOTween.To(
				() => CurrentTransposer.m_CameraDistance,//from
				(x) => CurrentTransposer.m_CameraDistance = x,//step
				currentZoom,//to
				0.25f);//t
		}

		private IEnumerator WaitWhileCamerasBlendes()
		{
			yield return null;
			yield return new WaitWhile(() => !brain.ActiveBlend?.IsComplete ?? false);

			CurrentTransposer = (brain.ActiveVirtualCamera as CinemachineVirtualCamera).GetCinemachineComponent<CinemachineFramingTransposer>();

			tacticCoroutine = null;
		}


		private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
		{
			leader = signal.leader.Model as CharacterModel;
			LookAt(leader);
		}

		private void OnStartDialogue(StartDialogueSignal signal)
		{
			AnimateZoom(zoomStandart);
			CameraToHome();
		}
	}
}