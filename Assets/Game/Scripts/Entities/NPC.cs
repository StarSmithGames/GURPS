using Game.Managers.CharacterManager;
using Game.Systems.BattleSystem;
using Game.Systems.SheetSystem;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Zenject;

namespace Game.Entities
{
	public class NPC : StubEntity
	{
		[SerializeField] private NPCData data;

		public override ISheet Sheet
		{
			get
			{
				if (npcSheet == null)
				{
					npcSheet = new NPCSheet(data);
				}

				return npcSheet;
			}
		}
		private NPCSheet npcSheet;


		protected FieldOfView fov;
		protected CharacterManager characterManager;
		protected BattleSystem battleSystem;

		[Inject]
		private void Construct(FieldOfView fov,
			CharacterManager characterManager,
			BattleSystem battleSystem)
		{
			this.fov = fov;
			this.characterManager = characterManager;
			this.battleSystem = battleSystem;
		}

		protected override IEnumerator Start()
		{
			fov.StartView();

			yield return base.Start();
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
	}
}