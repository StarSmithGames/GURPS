using Game.Systems.BattleSystem;
using UnityEngine.Events;

using Zenject;

namespace Game.Entities
{
	public class HumanoidEntity : Entity, IBattlable
	{
		public event UnityAction onBattleStateChanged;

		public bool InBattle => CurrentBattle != null;
		public bool InAction => AnimatorControl.IsAnimationProcess || IsHasTarget;

		public Battle CurrentBattle { get; private set; }

		public virtual bool JoinBattle(Battle battle)
		{
			if (CurrentBattle != null)
			{
				CurrentBattle.onBattleStateChanged -= OnBattleStateChanged;
				CurrentBattle.onBattleUpdated -= OnBattleUpdated;
			}
			CurrentBattle = battle;
			CurrentBattle.onBattleStateChanged += OnBattleStateChanged;
			CurrentBattle.onBattleUpdated += OnBattleUpdated;

			onBattleStateChanged?.Invoke();

			return true;
		}

		public virtual bool LeaveBattle()
		{
			if (CurrentBattle != null)
			{
				CurrentBattle.onBattleStateChanged -= OnBattleStateChanged;
				CurrentBattle.onBattleUpdated -= OnBattleUpdated;
			}
			CurrentBattle = null;

			onBattleStateChanged?.Invoke();

			return true;
		}

		public virtual void Attack() { }

		protected virtual void OnBattleStateChanged()
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
					case BattleState.EndBattle:
					{
						Markers.FollowMarker.Enable(false);

						break;
					}
				}
			}
			else
			{
				ResetMarkers();
			}
		}

		protected virtual void OnBattleUpdated() { }
	}
}