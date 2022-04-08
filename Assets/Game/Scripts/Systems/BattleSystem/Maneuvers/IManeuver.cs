using Game.Entities;

using UnityEngine;

namespace Game.Systems.BattleSystem
{
	public interface IManeuver
	{
		void Execute();
	}

	public abstract class Maneuver : IManeuver
	{
		public virtual void Execute()
		{

		}
	}

	public class Attack : Maneuver
	{
		private IEntity from;
		private IEntity to;

		public Attack(IEntity from, IEntity to)
		{
			this.from = from;
			this.to = to;
		}

		public override void Execute()
		{
			from.Sheet.Stats.ActionPoints.CurrentValue -= 1;
			(from as IBattlable).Attack(to);
		}
	}

	public class Inaction : Maneuver
	{
		private IEntity entity;

		public Inaction(IEntity entity)
		{
			this.entity = entity;
		}

		public override void Execute()
		{
			Debug.LogError($"{entity.GameObject.name} SKIP!");
		}
	}

	public class Wait : Maneuver
	{
		private IEntity entity;

		public Wait(IEntity entity)
		{
			this.entity = entity;
		}

		public override void Execute()
		{
			Debug.LogError($"{entity.GameObject.name} SKIP!");
		}
	}
}