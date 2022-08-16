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

	//IDamegeable, IKillable implementation
	//partial class StubEntityModel
	//{
	//	protected FloatingSystem floatingSystem;

	//	public override Damage GetDamage()
	//	{
	//		return new Damage()
	//		{
	//			amount = GetDamageFromTable(),
	//			damageType = DamageType.Crushing,
	//		};
	//	}

	//	public override void ApplyDamage<T>(T value)
	//	{
	//		if (value is Damage damage)
	//		{
	//			float dmg = (int)Mathf.Max(damage.DMG - 2, 0);

	//			if (dmg == 0)
	//			{
	//				floatingSystem.CreateText(transform.TransformPoint(CameraPivot.settings.startPosition), "Miss!", type: AnimationType.BasicDamageType);
	//			}
	//			else
	//			{
	//				floatingSystem.CreateText(transform.TransformPoint(CameraPivot.settings.startPosition), damage.damageType.ToString(), type: AnimationType.BasicDamageType);

	//				if (damage.IsPhysicalDamage)
	//				{

	//					floatingSystem.CreateText(transform.TransformPoint(CameraPivot.settings.startPosition), dmg.ToString(), type: AnimationType.AdvanceDamage);
	//					if (!Sheet.Settings.isImmortal)
	//					{
	//						Sheet.Stats.HitPoints.CurrentValue -= dmg;
	//					}
	//				}
	//				else if (damage.IsMagicalDamage)
	//				{

	//				}
	//			}
	//		}
	//	}
	//}
}