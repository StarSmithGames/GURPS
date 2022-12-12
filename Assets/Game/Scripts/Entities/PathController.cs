using CMF;

using Game.Managers.GameManager;
using Game.Managers.PartyManager;
using Game.Systems.BattleSystem;
using Game.Systems.NavigationSystem;
using Game.Systems.VFX;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.Entities
{
    public class PathController : IInitializable, IDisposable, ITickable
    {
		private ICharacter currentCharacter;
		private Transform currentCharacterTransform;
		private NavigationController currentCharacterNavigation;
		private bool isCharacterInBattle;
		private bool isCharacterTurnInBattle;
		private BattleExecutor lastBattle;
		private LinePathVFX linePath;

        private SignalBus signalBus;
		private LinePathVFX.Factory pathFactory;
		private DecalVFX targetDecal, folowDecal;
		private PartyManager partyManager;
		private TargetController targetController;

		public PathController(SignalBus signalBus,
			LinePathVFX.Factory pathFactory,
			[Inject(Id = "TargetDecal")] DecalVFX targetDecal,
			[Inject(Id = "FolowDecal")] DecalVFX folowDecal,
			PartyManager partyManager,
			TargetController targetController)
        {
            this.signalBus = signalBus;
			this.pathFactory = pathFactory;
			this.targetDecal = targetDecal;
			this.folowDecal = folowDecal;
			this.partyManager = partyManager;
			this.targetController = targetController;
		}

		public void Initialize()
		{
			targetDecal.Enable(false);
			folowDecal.Enable(false);

			signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
			signalBus?.Subscribe<SignalGameStateChanged>(OnGameStateChanged);

			targetController.onCastChanged += OnTargetControllerCastChanged;
		}

		public void Dispose()
		{
			signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
			signalBus?.Unsubscribe<SignalGameStateChanged>(OnGameStateChanged);

			targetController.onCastChanged -= OnTargetControllerCastChanged;
		}

		public void Tick()
		{
			if (currentCharacter == null) return;

			folowDecal.DrawDecal(currentCharacterTransform.position);
			targetDecal.DrawDecal(currentCharacterNavigation.CurrentNavMeshDestination);

			if (isCharacterInBattle)
			{
				if (isCharacterTurnInBattle)
				{
					if (currentCharacter.Model.IsHasTarget)
					{
						//range attack
					}
					else
					{
						if(linePath == null)
						{
							linePath = pathFactory.Create();
						}
						linePath.DrawLine(currentCharacterNavigation.CurrentPath.Path.ToArray());
					}
				}
			}
		}


		private void UpdateInBattle()
		{
			isCharacterInBattle = currentCharacter.Model.InBattle;
			isCharacterTurnInBattle = lastBattle?.CurrentInitiator == currentCharacter;
		}

		private void SetLeader(ICharacter character)
		{
			if(currentCharacter != null)
			{
				currentCharacter.Model.onBattleChanged -= OnBattleChanged;
				currentCharacter.Model.onDestinationChanged -= OnDestinationChanged;
				currentCharacter.Model.Controller.onReachedDestination -= OnCharacterReachedDestination;
			}

			currentCharacter = character;
			currentCharacterTransform = currentCharacter.Model.Transform;
			currentCharacterNavigation = currentCharacter.Model.Navigation;
			OnBattleChanged();

			if (currentCharacter != null)
			{
				currentCharacter.Model.onBattleChanged += OnBattleChanged;
				currentCharacter.Model.onDestinationChanged += OnDestinationChanged;
				currentCharacter.Model.Controller.onReachedDestination += OnCharacterReachedDestination;

				UpdateInBattle();
			}
		}


		private void FadeProps(bool trigger)
		{
			if (trigger)
			{
				if (isCharacterTurnInBattle && currentCharacter.Model.IsCanBattleMove)
				{
					linePath?.FadeTo(1);
					targetDecal.FadeTo(1, 0.25f);
				}
				else
				{
					if (linePath?.IsEnabled ?? false)
					{
						linePath.FadeTo(0);
					}

					if (targetDecal.IsEnabled)
					{
						targetDecal.FadeTo(0, 0.2f);
					}
				}
			}
			else
			{
				if (linePath?.IsEnabled ?? false)
				{
					linePath.FadeTo(0);
				}

				if (targetDecal.IsEnabled)
				{
					targetDecal.FadeTo(0, 0.2f);
				}
			}
		}

		private void OnTargetControllerCastChanged()
		{
			if (targetController.IsEnabled)
			{
				FadeProps(false);
			}
			else
			{
				FadeProps(true);
			}
		}

		private void OnDestinationChanged()
		{
			if (!isCharacterInBattle)
			{
				//Fade-In TargetMarker
				if (currentCharacter.Model.IsHasTarget)
				{
					if (!targetDecal.IsEnabled)
					{
						targetDecal.FadeTo(1f, 0.25f);
					}
				}
			}
			else
			{
				linePath.DrawLine(currentCharacterNavigation.CurrentPath.Path.ToArray());//redraw last choice destination
				linePath.SetMaterialSpeed(0);//stop line
			}
		}

		private void OnCharacterReachedDestination()
		{
			if (!isCharacterInBattle)
			{
				//Fade-Out TargetMarker
				if (targetDecal.IsEnabled)
				{
					targetDecal.FadeTo(0, 0.2f);
				}
			}
			else
			{
				linePath?.SetMaterialSpeedToDefault();
				if (currentCharacter.Model.IsCanBattleMove)
				{
					linePath?.DrawLine(currentCharacterNavigation.CurrentPath.Path.ToArray());//draw frame 
				}
				else
				{
					targetDecal.FadeTo(0, 0.2f);
				}
			}
		}


		private void OnBattleStateChanged(BattleExecutorState oldState, BattleExecutorState newState)
		{
			if (newState == BattleExecutorState.PreBattle)
			{
				folowDecal.Enable(true);
				targetDecal.Enable(false);
			}
			else if (newState == BattleExecutorState.EndBattle)
			{
				folowDecal.Enable(false);
				targetDecal.Enable(false);
				linePath?.DespawnIt();
				linePath = null;
			}
		}

		private void OnBattleOrderChanged(BattleOrder order)
		{
			if (order == BattleOrder.Turn)//update turn
			{
				isCharacterTurnInBattle = lastBattle.CurrentInitiator == currentCharacter.Model;

				FadeProps(true);
			}
		}

		private void OnBattleTurnSkipped(IBattlable skipper)
		{
			if(skipper == currentCharacter.Model)
			{
				FadeProps(false);
			}
		}

		private void OnBattleChanged()
		{
			if((currentCharacter.Model.CurrentBattle == null || lastBattle != currentCharacter.Model.CurrentBattle) && lastBattle != null)
			{
				lastBattle.onBattleOrderChanged -= OnBattleOrderChanged;
				lastBattle.onBattleStateChanged -= OnBattleStateChanged;
				lastBattle.onBattleTurnSkipped -= OnBattleTurnSkipped;
			}

			lastBattle = currentCharacter.Model.CurrentBattle;
			UpdateInBattle();

			if (lastBattle != null)
			{
				lastBattle.onBattleOrderChanged += OnBattleOrderChanged;
				lastBattle.onBattleStateChanged += OnBattleStateChanged;
				lastBattle.onBattleTurnSkipped += OnBattleTurnSkipped;
			}
		}

		private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
		{
			SetLeader(signal.leader);
		}

		private void OnGameStateChanged(SignalGameStateChanged signal)
		{
			if(signal.newGameState == GameState.Gameplay)
			{
				SetLeader(partyManager.PlayerParty.LeaderParty);
			}
		}
	}
}