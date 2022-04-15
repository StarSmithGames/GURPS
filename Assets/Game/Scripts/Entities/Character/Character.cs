using Game.Systems.BattleSystem;
using Game.Systems.DamageSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;
using DG.Tweening;

using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using Zenject;
using CMF;
using static UnityEngine.EventSystems.EventTrigger;

namespace Game.Entities
{
	public partial class Character : HumanoidEntity
	{
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

		public CharacterOutfit Outfit { get; private set; }

		public float CharacterRange => equipment.WeaponCurrent.Main.Item?.GetItemData<WeaponItemData>().weaponRange ?? 0f;

		public bool IsWithRangedWeapon { get; private set; }

		private IEquipment equipment;

		[Inject]
		private void Construct(CharacterOutfit outfit)
		{
			Outfit = outfit;

			equipment = (Sheet as CharacterSheet).Equipment;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if(equipment != null)
			{
				equipment.WeaponCurrent.onEquipWeaponChanged -= OnEquipWeaponChanged;
			}
		}

		protected override void Start()
		{
			base.Start();

			Controller.onReachedDestination += OnReachedDestination;
			equipment.WeaponCurrent.onEquipWeaponChanged += OnEquipWeaponChanged;
		}

		private void OnEquipWeaponChanged()
		{
			IsWithRangedWeapon = equipment.WeaponCurrent.Main.Item?.IsRangedWeapon ?? false;
		}
	}

	/// <summary>
	/// Override Battle & Animations implementation
	/// </summary>
	partial class Character
	{
		public override void SetTarget(Vector3 point, float maxPathDistance = -1)
		{
			base.SetTarget(point, InBattle ? Sheet.Stats.Move.CurrentValue : maxPathDistance);
		}

		public override void SetDestination(Vector3 destination, float maxPathDistance = -1)
		{
			base.SetDestination(destination, InBattle ? Sheet.Stats.Move.CurrentValue : maxPathDistance);

			if (!InBattle)
			{
				//Fade-In TargetMarker
				if (Controller.IsHasTarget)
				{
					if (!Markers.TargetMarker.IsEnabled)
					{
						Markers.TargetMarker.EnableIn();
					}
				}
			}
		}

		private void OnReachedDestination()
		{
			if (!InBattle)
			{
				//Fade-Out TargetMarker
				if (Markers.TargetMarker.IsEnabled)
				{
					Markers.TargetMarker.EnableOut();
				}
			}
		}

		protected override void OnBattleStateChanged()
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
		protected override void OnBattleUpdated()
		{
			bool isMineTurn = CurrentBattle.BattleFSM.CurrentTurn.Initiator == this && CurrentBattle.CurrentState != BattleState.EndBattle;

			Markers.LineMarker.Enable(InBattle && isMineTurn);
			Markers.TargetMarker.Enable(InBattle && isMineTurn);
		}
	}
}