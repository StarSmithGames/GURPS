using UnityEngine.Events;

namespace Game.Systems.DamageSystem
{
	public interface IKillable
	{
		public event UnityAction onDied;

		void Kill();
	}
}