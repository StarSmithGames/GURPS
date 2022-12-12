using Cinemachine;

using Game.Entities;
using Game.Entities.Models;
using Game.HUD;
using Game.Managers.CharacterManager;
using Game.Managers.GameManager;
using Game.Managers.InputManager;
using Game.Managers.PartyManager;
using Game.Systems.BattleSystem;
using Game.Systems.CombatDamageSystem;
using Game.Systems.ContextMenu;
using Game.Systems.DialogueSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;
using Game.Systems.TooltipSystem;
using Game.UI;
using Game.UI.CanvasSystem;

using System;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.CameraSystem
{
	/// <summary>
	/// TODO sub on battleSystem.CurrentExecutor
	/// </summary>
	public partial class CameraVisionLocation : CameraVision
	{
		private ICharacter leader;
		private ICharacterModel leaderModel;

		private WindowEntityInformation EntityInformation
		{
			get
			{
				if(entityInformation == null)
				{
					entityInformation = subCanvas.WindowsRegistrator.GetAs<WindowEntityInformation>();
				}

				return entityInformation;
			}
		}
		private WindowEntityInformation entityInformation;

		private CharacterManager characterManager;
		private PartyManager partyManager;
		private ContextMenuSystem contextMenuSystem;
		private UISubCanvas subCanvas;
		private BattleSystem.BattleSystem battleSystem;

		public CameraVisionLocation(SignalBus signalBus,
			CinemachineBrain brain,
			InputManager inputManager,
			UISubCanvas subCanvas,
			CharacterManager characterManager,
			PartyManager partyManager,
			GlobalSettings settings,
			ContextMenuSystem contextMenuSystem,
			BattleSystem.BattleSystem battleSystem) : base(signalBus, brain, inputManager)
		{
			this.subCanvas = subCanvas;
			this.characterManager = characterManager;
			this.partyManager = partyManager;
			this.settings = settings.cameraVisionLocation;
			this.contextMenuSystem = contextMenuSystem;
			this.battleSystem = battleSystem;
		}

		public override void Initialize()
		{
			base.Initialize();
			signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
		}

		public override void Dispose()
		{
			base.Dispose();
			signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
		}

		protected override void HandleHover(Vector3 point)
		{
			if (IsUI) return; 

			if (CurrentObserve != null)
			{
				if (CurrentObserve is ISheetable sheetable)
				{
					EntityInformation.SetSheet(sheetable.Sheet);

					if (!EntityInformation.IsShowing)
					{
						EntityInformation.Enable(true);
					}
				}
			}
			else
			{
				if (EntityInformation.IsShowing)
				{
					EntityInformation.Enable(false);
				}
			}
			

			if (leaderModel.InBattle)
			{
				HoverInBattle(point);
			}
		}

		protected override void HandleMouseClick(Vector3 point)
		{
			if (!leaderModel.IsInDialogue)
			{
				if (inputManager.IsLeftMouseButtonPressed())
				{
					if (!leaderModel.InBattle)
					{
						MouseClick(point);
					}
					else
					{
						MouseClickInBattle(point);
					}
				}
				else if (inputManager.IsRightMouseButtonPressed())
				{
					//ContextMenu
					if (inputManager.IsRightMouseButtonDown())
					{
						leaderModel.Stop();

						if (CurrentObserve != null)
						{
							contextMenuSystem.SetTarget(CurrentObserve);
						}
					}
				}
			}
		}

		private bool IsPointInLeaderRange(Vector3 point)
		{
			return (Vector3.Distance(leaderModel.Outfit.LeftHandPivot.position, point) <= leaderModel.CharacterRange);
		}


		private void ChangeLeader(ICharacter character)
		{
			leader = character;
			leaderModel = leader.Model;

			IsCanHoldMouse = leaderModel.InBattle ? false : settings.isCanHoldMouse;
		}

		protected override void OnHoverObserveChanged()
		{
			if (!leaderModel.IsInDialogue)
			{
				if (leaderModel.InBattle)
				{
					HoverObserveChangedInBattle();
				}
			}
		}

		private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
		{
			if (IsEnabled)
			{
				ChangeLeader(signal.leader);
			}
		}

		protected override void OnGameStateChanged(SignalGameStateChanged signal)
		{
			if (signal.newGameState == GameState.Gameplay)
			{
				ChangeLeader(partyManager.PlayerParty.LeaderParty);
			}

			base.OnGameStateChanged(signal);
		}
	}

	//Free implementation
	public partial class CameraVisionLocation
	{
		private void MouseClick(Vector3 point)
		{
			if (CurrentObserve != null)
			{
				//Interaction
				if (inputManager.IsLeftMouseButtonDown())
				{
					if(CurrentObserve is IActor actor)
					{
						Talker.ABTalk(leaderModel, actor);
					}
					else if (CurrentObserve is IInteractable interactable)
					{
						Interactor.ABInteraction(leaderModel, interactable);
					}
				}
			}
			else
			{
				//Targeting
				if (IsCanHoldMouse || inputManager.IsLeftMouseButtonDown())
				{
					if (!IsUI)
					{
						if (IsMouseHit)
						{
							leaderModel.SetDestination(point);
						}
						else
						{
							leaderModel.SetDestination(leaderModel.Navigation.CurrentNavMeshDestination);
						}
					}
				}
			}
		}
	}


	//In Battle implementation
	public partial class CameraVisionLocation
	{
		private void HoverInBattle(Vector3 point)
		{
			if (!leaderModel.IsHasTarget)
			{
				if (CurrentObserve != null)
				{
					if (leaderModel.IsWithRangedWeapon && CurrentObserve != leaderModel)
					{

					}
					else if (CurrentObserve is ICombatable combatable)
					{
						leaderModel.SetTarget(combatable.BattlePoint.GetIteractionPosition(leaderModel), leaderModel.Sheet.Stats.Move.CurrentValue);
					}
				}
				else
				{
					leaderModel.SetTarget(point, leaderModel.Sheet.Stats.Move.CurrentValue);

					//if (leader.IsRangeAttackTest)
					//{
					//	leader.Controller.RotateTo(point);
					//}
				}
			}
		}

		private void MouseClickInBattle(Vector3 point)
		{
			if (battleSystem.CurrentExecutor.CurrentState == BattleExecutorState.Battle)
			{
				if (!(IsMouseHit && !IsUI)) return;

				if (CurrentObserve != null)
				{
					if (CurrentObserve is IInteractable interactable)
					{
						bool isCanReach = (leader.Sheet.Stats.Move.CurrentValue - leaderModel.Navigation.FullPath.Distance) >= 0 && leader.Sheet.Stats.Move.CurrentValue != 0;

						if (isCanReach || interactable.InteractionPoint.IsInRange(leaderModel.Transform.position))
						{
							Interactor.ABInteraction(leaderModel, interactable);
						}
						else//Try reach target
						{
							SetDestination();
						}
					}
				}
				else//Free space
				{
					SetDestination();
				}
			}

			void SetDestination()
			{
				//if (!leader.TaskSequence.IsSequenceProcess || leader.TaskSequence.IsCanBeBreaked)
				if (IsCanHoldMouse || inputManager.IsLeftMouseButtonDown())
				{
					if (battleSystem.CurrentExecutor.IsPlayerTurn && battleSystem.CurrentExecutor.InitiatorCanAct)
					{
						if (!leaderModel.IsHasTarget && leaderModel.IsCanBattleMove)
						{
							leaderModel.SetDestination(point, leaderModel.Sheet.Stats.Move.CurrentValue);
						}
					}
				}
			}
		}

		private void HoverObserveChangedInBattle()
		{
			if (CurrentObserve == null || CurrentObserve == leaderModel)
			{
			}
			else
			{
				if (leaderModel.IsWithRangedWeapon)
				{
					leaderModel.SetTarget(leaderModel.Transform.position);//hide target
				}
			}
		}
	}
}