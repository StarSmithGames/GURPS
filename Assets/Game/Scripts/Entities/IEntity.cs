using Game.Entities.Models;

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