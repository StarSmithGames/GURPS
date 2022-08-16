using Game.Systems.DamageSystem;
using Game.Systems.SheetSystem;

using UnityEngine;
using UnityEngine.Events;

namespace Game.Entities
{
	public interface IEntity : ISheetable, IDamegeable, IKillable
	{
		IEntityModel Model { get; }
	}

	public partial class Entity : IEntity
	{
		public virtual ISheet Sheet { get; protected set; }
		public virtual IEntityModel Model { get; protected set; }
	}

	//IDamegeable, IKillable implementation
	public partial class Entity
	{
		public event UnityAction<IEntity> onDied;

		public virtual void Kill()
		{
			//Controller.Enable(false);
			onDied?.Invoke(this);
		}

		public virtual Damage GetDamage()
		{
			return null;
		}

		public virtual void ApplyDamage<T>(T value) { }

		protected Vector2 GetDamageFromTable()
		{
			return new Vector2(1, 7);
		}
	}
}