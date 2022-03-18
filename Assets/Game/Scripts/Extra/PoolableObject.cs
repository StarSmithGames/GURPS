using UnityEngine;
using Zenject;

public interface IPoolable
{
	IMemoryPool Pool { get; }

	void DespawnIt();
}

public class PoolableObject : MonoBehaviour, IPoolable, IPoolable<IMemoryPool>
{
	public IMemoryPool Pool { get => pool; protected set => pool = value; }
	private IMemoryPool pool;

	public void DespawnIt()
	{
		pool?.Despawn(this);
	}

	public void OnSpawned(IMemoryPool pool)
	{
		this.pool = pool;
	}

	public void OnDespawned()
	{
		pool = null;
	}
}