using Game.Managers.GameManager;
using Game.Systems.BattleSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.InventorySystem;
using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Entities
{
	public class Character : Entity, IBattlable
	{
		public UnityAction onCharacterUpdated;

		public CharacterSheet CharacterSheet
		{
			get
			{
				if (characterSheet == null)
				{
					characterSheet = new CharacterSheet(EntityData);
				}

				return characterSheet;
			}
		}
		private CharacterSheet characterSheet;

		public bool InBattle => CurrentBattle != null;

		public Battle CurrentBattle { get; private set; }

		private void OnDestroy()
		{
			if (Controller != null)
			{
				Controller.onTargetChanged -= OnTargetChanged;
			}
		}

		protected override void Start()
		{
			base.Start();
			Controller.onTargetChanged += OnTargetChanged;
		}

		public void InteractWith(IObservable observable)
		{
			switch (observable)
			{
				case IInteractable interactable:
				{
					interactable.InteractFrom(this);
					break;
				}
			}
		}

		public bool JoinBattle(Battle battle)
		{
			if(CurrentBattle != null)
			{
				CurrentBattle.onBattleStateChanged -= OnBattleStateChanged;
				CurrentBattle.onBattleUpdated -= OnBattleUpdated;
			}
			CurrentBattle = battle;
			CurrentBattle.onBattleStateChanged += OnBattleStateChanged;
			CurrentBattle.onBattleUpdated += OnBattleUpdated;

			onCharacterUpdated?.Invoke();

			return true;
		}

		public bool LeaveBattle()
		{
			if(CurrentBattle != null)
			{
				CurrentBattle.onBattleStateChanged -= OnBattleStateChanged;
			}
			CurrentBattle = null;

			onCharacterUpdated?.Invoke();

			return true;
		}


		private void OnTargetChanged()
		{
			if (!InBattle)
			{
				//Fade-In Fade-Out TargetMarker
				if (Controller.IsHasTarget)
				{
					if (!Markers.TargetMarker.IsEnabled)
					{
						Markers.TargetMarker.EnableIn();
					}
				}
				else
				{
					if (Markers.TargetMarker.IsEnabled)
					{
						Markers.TargetMarker.EnableOut();
					}
				}
			}
		}

		private void OnBattleUpdated()
		{
			bool isMineTurn = CurrentBattle.BattleFSM.CurrentTurn.Initiator == this && CurrentBattle.CurrentState != BattleState.EndBattle;

			Markers.LineMarker.Enable(InBattle && isMineTurn);
			Markers.TargetMarker.Enable(InBattle && isMineTurn);
		}

		private void OnBattleStateChanged()
		{
			if (InBattle)
			{
				switch (CurrentBattle.CurrentState)
				{
					case BattleState.PreBattle:
					{
						Markers.FollowMarker.Enable(true);

						Markers.TargetMarker.Enable(false);

						Markers.AreaMarker.Enable(false);

						Markers.LineMarker.Enable(false);
						break;
					}
					case BattleState.Battle:
					{
						Markers.FollowMarker.Enable(true);

						Markers.TargetMarker.Enable(true);

						Markers.AreaMarker.Enable(false);

						Markers.LineMarker.Enable(true);
						break;
					}
					case BattleState.EndBattle:
					{
						Markers.FollowMarker.Enable(false);

						Markers.TargetMarker.Enable(true);

						Markers.AreaMarker.Enable(false);

						Markers.LineMarker.Enable(false);
						break;
					}
				}
			}
			else
			{
				ResetMarkers();
			}
		}
	}


	public class CharacterSheet : EntitySheet
	{
		public virtual IEquipment Equipment { get; private set; }

		public CharacterSheet(EntityData data) : base(data)
		{
			Equipment = new Equipment(Inventory);
		}
	}
}