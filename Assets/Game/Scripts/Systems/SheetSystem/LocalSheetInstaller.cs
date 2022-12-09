using Game.Entities;
using Game.Systems.SheetSystem.Skills;
using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem
{
	[CreateAssetMenu(fileName = "LocalSheetInstaller", menuName = "Installers/LocalSheetInstaller")]
	public class LocalSheetInstaller : ScriptableObjectInstaller
	{
		public override void InstallBindings()
		{
			Container.BindFactory<PassiveSkillData, ICharacter, PassiveSkill, PassiveSkill.Factory>();

			Container
				.BindFactory<BlitzBoltData, ICharacter, BlitzBoltSkill, BlitzBoltSkill.Factory>()
				.FromNewComponentOnNewGameObject().AsSingle().NonLazy();
			Container
				.BindFactory<ScorchingRayData, ICharacter, ScorchingRaySkill, ScorchingRaySkill.Factory>()
				.FromNewComponentOnNewGameObject().AsSingle().NonLazy();
			//Container
			//	.BindFactory<ICharacter, StunSkill, StunSkill.Factory>()
			//	.FromNewComponentOnNewGameObject().AsSingle().NonLazy();
		}
	}
}