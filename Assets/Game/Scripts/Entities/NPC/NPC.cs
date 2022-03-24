using Game.Managers.GameManager;
using Game.Systems.BattleSystem;

using System.Collections.Generic;

using Zenject;
using Game.Managers.CharacterManager;

namespace Game.Entities
{
	public class NPC : Entity
	{
		private FieldOfView fov;
		private UIManager uiManager;
		private GameManager gameManager;
		private CharacterManager characterManager;
		private BattleSystem battleSystem;

		[Inject]
		private void Construct(
			FieldOfView fov,
			UIManager uiManager,
			GameManager gameManager,
			CharacterManager characterManager,
			BattleSystem battleSystem)
		{
			this.fov = fov;

			this.uiManager = uiManager;
			this.gameManager = gameManager;
			this.characterManager = characterManager;
			this.battleSystem = battleSystem;

			fov.StartView();
		}

		private void Update()
		{
			if (gameManager.CurrentGameState != GameState.PreBattle &&
				gameManager.CurrentGameState != GameState.Battle)
			{
				if (fov.visibleTargets.Count > 0)
				{
					fov.StopView();
					List<IEntity> entities = new List<IEntity>();

					characterManager.Party.Characters.ForEach((x) =>
					{
						entities.Add(x);
					});
					entities.Add(this);

					battleSystem.StartBattle(entities);
				}
			}
		}

		public override void StartObserve()
		{
			base.StartObserve();
			uiManager.Battle.ShowEntityInformation(this);
		}

		public override void EndObserve()
		{
			base.EndObserve();
			uiManager.Battle.HideEntityInformation();
		}
	}
}