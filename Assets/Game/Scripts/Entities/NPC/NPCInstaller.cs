using UnityEngine;
using UnityEngine.AI;

using Zenject;

namespace Game.Entities
{
	public class NPCInstaller : EntityInstaller
	{
		[Space]
		[SerializeField] private FieldOfView fov;

		public override void InstallBindings()
		{
			base.InstallBindings();
			Container.BindInstance(fov);
		}
	}
}