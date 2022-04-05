using Game.Systems.CutomizationSystem;

using UnityEngine;

namespace Game.Entities
{
    public class CharacterInstaller : EntityInstaller
    {
		[Space]
		[SerializeField] private CharacterAnimatorControl animatorControl;
        [SerializeField] private CharacterOutfit outfit;

		public override void InstallBindings()
		{
			base.InstallBindings();
			Container.BindInstance(animatorControl);
			Container.BindInstance(outfit);
		}
	}
}