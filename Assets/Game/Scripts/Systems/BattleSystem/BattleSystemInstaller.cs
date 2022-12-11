using Game.Systems.BattleSystem.TargetSystem;

using System.Collections.Generic;

using UnityEngine;

using Zenject;

namespace Game.Systems.BattleSystem
{
	[CreateAssetMenu(fileName = "BattleSystemInstaller", menuName = "Installers/BattleSystemInstaller")]
	public class BattleSystemInstaller : ScriptableObjectInstaller<BattleSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<SignalCurrentBattleExecutorChanged>();

			Container.BindFactory< List <IBattlable>, Battle, Battle.Factory>().WhenInjectedInto<BattleExecutor>();
			Container.BindFactory<BattleExecutor.Settings, BattleExecutor, BattleExecutor.Factory>().WhenInjectedInto<BattleSystem>();
			Container.BindInterfacesAndSelfTo<BattleSystem>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<TargetController>().AsSingle().NonLazy();
		}
	}

	public enum BattleExecutorState
	{
		Initialization,
		PreBattle,
		Battle,
		EndBattle,
	}

	public enum BattleOrder
	{
		Recovery,
		Turn,
	}
}