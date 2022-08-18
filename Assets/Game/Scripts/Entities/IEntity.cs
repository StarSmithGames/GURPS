using Game.Entities.Models;

using Zenject;

namespace Game.Entities
{
	public interface IEntity
	{
		IEntityModel Model { get; }
	}

	public abstract class Entity : IEntity
	{
		public virtual IEntityModel Model { get; protected set; }
	}
}