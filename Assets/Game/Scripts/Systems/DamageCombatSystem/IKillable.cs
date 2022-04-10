using Game.Entities;

using UnityEngine.Events;

namespace Game.Systems.DamageSystem
{
	public interface IKillable
	{
		public event UnityAction<IEntity> onDied;

		void Kill();
	}
}