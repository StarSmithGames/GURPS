using Game.Systems.BattleSystem;
using Game.Systems.DamageSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Entities
{
	public partial class Character : Entity, IBattlable
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

		public override void TryInteractWith(IInteractable interactable)
		{
			lastInteractable = interactable;
			if (interactable is IEntity)
			{
				if (InBattle)
				{
					interactable.InteractFrom(this, InternalInteraction());
					return;
				}
			}

			interactable.InteractFrom(this);
		}

		protected override IEnumerator InternalInteraction()
		{
			if (InBattle)
			{
				if(lastInteractable is IEntity entity)
				{
					if (!CurrentBattle.BattleFSM.CurrentTurn.ContainsManeuver<Attack>())
					{
						CurrentBattle.BattleFSM.CurrentTurn.AddManeuver(new Attack(this, entity));
					}
					else
					{
						Debug.LogError("Not Enough actions");
					}
				}
			}
			yield return null;
		}

		public override Damage GetDamage()
		{
			CharacterSheet sheet = Sheet as CharacterSheet;

			switch (sheet.Equipment.WeaponCurrent.Hands)
			{
				case Hands.None:
				{
					return base.GetDamage();
				}
				case Hands.Main:
				{
					return base.GetDamage();
				}
				case Hands.Spare:
				{
					return base.GetDamage();
				}
				case Hands.Both:
				{
					return base.GetDamage();
				}
			}

			return base.GetDamage();
		}
	}

	/// <summary>
	/// Override Battle & Animations implementation
	/// </summary>
	partial class Character
	{
		public bool InBattle => CurrentBattle != null;
		public Battle CurrentBattle { get; private set; }

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


		public void Attack(IEntity entity)
		{
			CharacterSheet sheet = Sheet as CharacterSheet;

			if (sheet.Equipment.WeaponCurrent.Hands == Hands.None)
			{
				Attack(0, Random.Range(0, 3));
			}
			else
			{
				Attack(1, 0);
			}
		}

		public void Attack(int weaponType = 0, int attackType = 0)
		{
			(AnimatorControl as CharacterAnimatorControl).Attack(weaponType, attackType);
		}

		
		public bool JoinBattle(Battle battle)
		{
			if (CurrentBattle != null)
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
			if (CurrentBattle != null)
			{
				CurrentBattle.onBattleStateChanged -= OnBattleStateChanged;
				CurrentBattle.onBattleUpdated -= OnBattleUpdated;
			}
			CurrentBattle = null;

			onCharacterBattleStateChanged?.Invoke();

			return true;
		}


		protected override void SubscribeAnimationEvents()
		{
			var current = (AnimatorControl as CharacterAnimatorControl);

			current.onAttackEvent += OnAttacked;

			current.onAttackLeftHand += OnAttacked;
			current.onAttackRightHand += OnAttacked;
			current.onAttackKick += OnAttacked;
		}
		protected override void UnSubscribeAnimationEvents()
		{
			var current = (AnimatorControl as CharacterAnimatorControl);

			if (current != null)
			{
				current.onAttackEvent -= OnAttacked;

				current.onAttackLeftHand -= OnAttacked;
				current.onAttackRightHand -= OnAttacked;
				current.onAttackKick -= OnAttacked;
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