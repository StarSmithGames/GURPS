using Game.Managers.CharacterManager;
using Game.Managers.GameManager;

using UnityEngine;

using Zenject;

public class ProjectInstaller : MonoInstaller
{
	[SerializeField] private GlobalSettings globalSettings;

	public override void InstallBindings()
	{
		SignalBusInstaller.Install(Container);

		GameManagerInstaller.Install(Container);

		Container.BindInstance(globalSettings);

		CharacterManagerInstaller.Install(Container);
	}
}