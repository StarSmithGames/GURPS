using Game.Systems.BattleSystem;
using Game.Systems.CameraSystem;
using Game.Systems.DialogueSystem;
using Game.Systems.SheetSystem.Abilities;

using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

namespace Game.Entities
{
    public class CharacterInstaller : EntityInstaller
    {
		[Title("Character")]
		[SerializeField] private CameraPivot cameraPivot;
		[SerializeField] private Markers markers;
		[SerializeField] private CharacterOutfit outfit;
		[Space]
		[SerializeField] private Barker barker;

		public override void InstallBindings()
		{
			base.InstallBindings();

			Container.DeclareSignal<SignalJoinBattleLocal>();
			Container.DeclareSignal<SignalLeaveBattleLocal>();

			Container.BindInstance(cameraPivot);
			Container.BindInstance(markers);
			Container.BindInstance(outfit);
			Container.BindInstance(barker);

			Container.BindInterfacesTo<AbilityProvider>().AsSingle().NonLazy();
		}
	}
}