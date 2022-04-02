using UnityEngine;

namespace Game.Entities
{
    public class CharacterInstaller : EntityInstaller
    {
		[Space]
		[SerializeField] private Character character;

		public override void InstallBindings()
		{
			base.InstallBindings();

			Container.BindInstance(character);
		}
	}
}