using UnityEngine;

using Zenject;

namespace Game.Systems.BattleSystem
{
	[CreateAssetMenu(fileName = "BattleSystemInstaller", menuName = "Installers/BattleSystemInstaller")]
	public class BattleSystemInstaller : ScriptableObjectInstaller<BattleSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<SignalCurrentBattleChanged>();

			Container.BindInterfacesAndSelfTo<BattleSystem>().AsSingle();
		}
	}
}