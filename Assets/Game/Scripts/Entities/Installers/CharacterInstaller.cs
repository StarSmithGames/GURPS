using UnityEngine;

namespace Game.Entities
{
    public class CharacterInstaller : EntityInstaller
    {
		[Space]
        [SerializeField] private CharacterOutfit outfit;

		public override void InstallBindings()
		{
			base.InstallBindings();
			Container.BindInstance(outfit);
		}
	}
}