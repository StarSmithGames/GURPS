using Game.Managers.CharacterManager;
using Game.Systems.BattleSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Entities
{
	public class BattlableNPC : NPC, IBattlable
	{
		public bool InBattle => CurrentBattle != null;
		public bool InAction => false;

		public Battle CurrentBattle { get; private set; }

		private CharacterManager characterManager;
		private BattleSystem battleSystem;

		[Inject]
		private void Construct(
			CharacterManager characterManager,
			BattleSystem battleSystem)
		{
			this.characterManager = characterManager;
			this.battleSystem = battleSystem;
		}

		protected override void Start()
		{
			base.Start();
			fov.StartView();
		}

		private void Update()
		{
			if (fov.IsViewProccess)
			{
				if (!InBattle)
				{
					if (fov.visibleTargets.Count > 0)
					{
						fov.StopView();
						List<IBattlable> entities = new List<IBattlable>();

						for (int i = 0; i < characterManager.CurrentParty.Characters.Count; i++)
						{
							entities.Add(characterManager.CurrentParty.Characters[i]);
						}
						entities.Add(this);

						battleSystem.StartBattle(entities);
					}
				}
			}
		}


		protected override void ResetMarkers()
		{
			Markers.FollowMarker.Enable(false);

			Markers.TargetMarker.Enable(false);

			Markers.AreaMarker.Enable(false);

			Markers.LineMarker.Enable(false);
		}

		public bool JoinBattle(Battle battle)
		{
			if (CurrentBattle != null)
			{
				CurrentBattle.onBattleStateChanged -= OnBattleStateChanged;
			}
			CurrentBattle = battle;
			CurrentBattle.onBattleStateChanged += OnBattleStateChanged;

			return true;
		}

		public bool LeaveBattle()
		{
			if (CurrentBattle != null)
			{
				CurrentBattle.onBattleStateChanged -= OnBattleStateChanged;
			}
			CurrentBattle = null;

			return true;
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
						break;
					}
				}
			}
			else
			{
				ResetMarkers();
			}
		}

		public void Attack(IEntity entity)
		{
		}
	}
}