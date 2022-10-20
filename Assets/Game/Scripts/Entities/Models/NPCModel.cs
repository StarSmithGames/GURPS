using Game.Managers.CharacterManager;
using Game.Systems.BattleSystem;

using System.Collections;

using Zenject;

namespace Game.Entities.Models
{
	public interface INPCModel : ICharacterModel { }

	public class NPCModel : CharacterModel, INPCModel
	{
		public CharacterData data;

		public override ICharacter Character
		{
			get
			{
				if(character == null)
				{
					character = null;//characterManager.GetCharacter(data, this);
				}

				return character;
			}
		}
		private ICharacter character;

		private CharacterManager characterManager;
		protected FieldOfView fov;
		protected BattleSystem battleSystem;

		[Inject]
		private void Construct(/*FieldOfView fov, */CharacterManager characterManager, BattleSystem battleSystem)
		{
			//this.fov = fov;
			this.characterManager = characterManager;
			this.battleSystem = battleSystem;
		}

		protected override IEnumerator Start()
		{
			//fov.StartView();

			yield return base.Start();
		}

		//private void Update()
		//{
			//if (fov.IsViewProccess)
			//{
			//	if (!InBattle)
			//	{
			//		if (fov.visibleTargets.Count > 0)
			//		{
			//			fov.StopView();
			//			List<IBattlable> entities = new List<IBattlable>();

			//			//for (int i = 0; i < characterManager.CurrentParty.Characters.Count; i++)
			//			//{
			//			//	entities.Add(characterManager.CurrentParty.Characters[i]);
			//			//}
			//			entities.Add(this);

			//			battleSystem.StartBattle(entities);
			//		}
			//	}
			//}
		//}

		protected override void ResetMarkers()
		{
			Markers.FollowMarker.Enable(false);

			Markers.TargetMarker.Enable(false);

			Markers.AreaMarker.Enable(false);

			Markers.LineMarker.Enable(false);
		}
	}
}