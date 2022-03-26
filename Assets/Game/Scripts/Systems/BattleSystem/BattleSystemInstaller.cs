using Zenject;

namespace Game.Systems.BattleSystem
{
	public class BattleSystemInstaller : Installer<BattleSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<SignalCurrentBattleChanged>();

			Container.BindInterfacesAndSelfTo<BattleManager>().AsSingle();
			Container.BindInterfacesAndSelfTo<BattleSystem>().AsSingle();
		}
	}
}