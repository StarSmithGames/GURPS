using Zenject;

namespace Game.Systems.BattleSystem
{
	public class BattleSystemInstaller : Installer<BattleSystemInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<BattleSystem>().AsSingle();
		}
	}
}