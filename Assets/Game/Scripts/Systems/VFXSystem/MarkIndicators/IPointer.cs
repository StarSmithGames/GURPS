using UnityEngine;

namespace Game.Systems.VFX
{
	public interface IPointer : IPoolable
	{
		Transform Transform { get; }
	}

	public class BasePointer : PoolableObject, IPointer
	{
		public Transform Transform => transform;
	}
}