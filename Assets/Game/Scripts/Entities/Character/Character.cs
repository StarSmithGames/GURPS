using Game.Systems.BattleSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Entities
{
	public class Character : Entity, IBattlable
	{
		public UnityAction onCharacterBattleStateChanged;

		[SerializeField] private CharacterData data;

		public override ISheet Sheet
		{
			get
			{
				if(characterSheet == null)
				{
					characterSheet = new CharacterSheet(data);
				}

				return characterSheet;
			}
		}
		private CharacterSheet characterSheet;

		public bool InBattle => CurrentBattle != null;

		public Battle CurrentBattle { get; private set; }

		private CharacterAnimatorControl animatorControl;

		[Inject]
		private void Construct(CharacterAnimatorControl animatorControl)
		{
			this.animatorControl = animatorControl;
		}

		public override void TryInteractWith(IInteractable interactable)
		{
			lastInteractable = interactable;
			if (interactable is IEntity entity)
			{
				if (InBattle)
				{
					interactable.InteractFrom(this, InternalInteraction());
				}
				else
				{
					interactable.InteractFrom(this);
				}
			}
			else
			{
				interactable.InteractFrom(this);
			}
		}

		protected override IEnumerator InternalInteraction()
		{
			if (InBattle)
			{
				var i = Random.Range(0, 100);
				Attack(i < 60 ? 0 : 1);
			}
			yield return null;
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

			onCharacterBattleStateChanged?.Invoke();

			return true;
		}

		public bool LeaveBattle()
		{
			if(CurrentBattle != null)
			{
				CurrentBattle.onBattleStateChanged -= OnBattleStateChanged;
			}
			CurrentBattle = null;

			onCharacterBattleStateChanged?.Invoke();

			return true;
		}

		public override void SetTarget(Vector3 point, float maxPathDistance = -1)
		{
			base.SetTarget(point, InBattle ? Sheet.Stats.Move.CurrentValue : maxPathDistance);
		}

		public override void SetDestination(Vector3 destination, float maxPathDistance = -1)
		{
			base.SetDestination(destination, InBattle ? Sheet.Stats.Move.CurrentValue : maxPathDistance);

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
}