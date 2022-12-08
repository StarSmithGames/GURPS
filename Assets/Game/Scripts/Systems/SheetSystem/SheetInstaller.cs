using Game.Entities;
using Game.Entities.Models;
using Game.Systems.SheetSystem.Skills;
using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem
{
	[CreateAssetMenu(fileName = "SheetInstaller", menuName = "Installers/SheetInstaller")]
	public class SheetInstaller : ScriptableObjectInstaller
	{
		public override void InstallBindings()
		{
			Container.BindFactory<PassiveSkillData, ICharacter, PassiveSkill, PassiveSkill.Factory>();

			Container
				.BindFactory<BlitzBoltData, ICharacter, BlitzBoltSkill, BlitzBoltSkill.Factory>()
				.FromNewComponentOnNewGameObject().AsSingle().NonLazy();
			//Container
			//	.BindFactory<ICharacter, StunSkill, StunSkill.Factory>()
			//	.FromNewComponentOnNewGameObject().AsSingle().NonLazy();
		}
	}
}