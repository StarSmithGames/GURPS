using Game.Entities;
using Game.Systems.SheetSystem.Skills;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem
{
	public class SheetInstaller : MonoInstaller<SheetInstaller>
	{
		[SerializeField] private BlitzBoltSkill blitzBoltSkill;
		[SerializeField] private StunSkill stunSkill;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<ActiveSkillsRegistrator>().AsSingle().NonLazy();
			Container
				.BindFactory<BlitzBoltSkill, BlitzBoltSkill.Factory>()
				.FromComponentInNewPrefab(blitzBoltSkill).AsSingle().NonLazy();
			Container
				.BindFactory<StunSkill, StunSkill.Factory>()
				.FromComponentInNewPrefab(stunSkill).AsSingle().NonLazy();
		}
	}
}