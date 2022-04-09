using Game.Entities;

using System.Collections.Generic;
using System.Linq;

using UnityEngine.Events;

namespace Game.Systems.SheetSystem
{
	public interface IConditions
	{
		public event UnityAction onConditionsChanged;

		List<ICondition> CurrentConditions { get; }

		bool Add<T>(T condition) where T : ICondition;
		bool Remove<T>(T condition) where T : ICondition;

		bool IsContains<T>(T condition) where T : ICondition;
		bool IsContains<T>() where T : ICondition;
	}

	public class Conditions : IConditions
	{
		public event UnityAction onConditionsChanged;

		public List<ICondition> CurrentConditions { get; private set; }

		public Conditions()
		{
			CurrentConditions = new List<ICondition>();
		}

		public bool IsContains<T>(T condition) where T : ICondition
		{
			return CurrentConditions.Contains(condition);
		}
		public bool IsContains<T>() where T : ICondition
		{
			return CurrentConditions.OfType<T>().Any();
		}

		public bool Add<T>(T condition) where T : ICondition
		{
			if (!IsContains(condition) && !IsContains<T>())
			{
				CurrentConditions.Add(condition);

				onConditionsChanged?.Invoke();

				return true;
			}

			return false;
		}

		public bool Remove<T>(T condition) where T : ICondition
		{
			if (IsContains(condition))
			{
				CurrentConditions.Remove(condition);

				onConditionsChanged?.Invoke();

				return true;
			}

			return false;
		}
	}


	public interface ICondition { }

	public class Death : ICondition { }

	public class Unconscious : ICondition
	{
		public Unconscious(IEntity entity)
		{

		}
	}

	public class Stun : ICondition
	{
		public int turns = 3;

		public Stun(IEntity entity)
		{

		}
	}
}