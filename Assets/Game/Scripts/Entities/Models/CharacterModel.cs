using Game.Systems.BattleSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using UnityEngine;

using Zenject;
using System.Collections;
using Game.Managers.CharacterManager;

namespace Game.Entities.Models
{
	public interface ICharacterModel
	{
		CharacterModel.Data GetData();
	}

	public partial class CharacterModel : StubEntityModel, ICharacterModel
	{
		public CharacterData data;

		public CharacterOutfit Outfit { get; private set; }

		public float CharacterRange => equipment.WeaponCurrent.Main.Item?.GetItemData<WeaponItemData>().weaponRange ?? 0f;

		public bool IsWithRangedWeapon { get; private set; }

		private IEquipment equipment;

		protected CharacterManager characterManager;

		[Inject]
		private void Construct(CharacterOutfit outfit, CharacterManager characterManager)
		{
			Outfit = outfit;
			this.characterManager = characterManager;

			//equipment = (Sheet as CharacterSheet).Equipment;
		}

		protected override IEnumerator Start()
		{
			characterManager.Registrate(this);

			Controller.onReachedDestination += OnReachedDestination;
			equipment.WeaponCurrent.onEquipWeaponChanged += OnEquipWeaponChanged;

			yield return base.Start();
		}

		protected override void OnDestroy()
		{
			characterManager.UnRegistrate(this);

			base.OnDestroy();

			if (equipment != null)
			{
				equipment.WeaponCurrent.onEquipWeaponChanged -= OnEquipWeaponChanged;
			}
		}

		#region Observe
		public override void StartObserve()
		{
			base.StartObserve();
			//uiManager.Battle.SetSheet(Sheet);
		}
		public override void EndObserve()
		{
			base.EndObserve();
			//uiManager.Battle.SetSheet(null);
		}
		#endregion

		private void OnEquipWeaponChanged()
		{
			IsWithRangedWeapon = equipment.WeaponCurrent.Main.Item?.IsRangedWeapon ?? false;
		}

		public Data GetData()
		{
			return new Data()
			{
				transform = new DefaultTransform()
				{
					position = transform.position,
					rotation = transform.rotation,
					scale = transform.localScale,
				}
			};
		}

		public struct Data
		{
			public DefaultTransform transform;
		}
	}

	/// <summary>
	/// Override Battle & Animations implementation
	/// </summary>
	partial class CharacterModel
	{
		public override void SetTarget(Vector3 point, float maxPathDistance = -1)
		{
			//base.SetTarget(point, InBattle ? Sheet.Stats.Move.CurrentValue : maxPathDistance);
		}

		public override void SetDestination(Vector3 destination, float maxPathDistance = -1)
		{
			//base.SetDestination(destination, InBattle ? Sheet.Stats.Move.CurrentValue : maxPathDistance);

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

		public override void Stop()
		{
			base.Stop();

			OnReachedDestination();
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