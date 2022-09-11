using System.Collections.Generic;
using System.Linq;

using UnityEngine.Events;

public class Registrator<T>
{
	public event UnityAction onCollectionChanged;


	public List<T> registers;

	public Registrator()
	{
		registers = new List<T>();
	}


	public virtual void Registrate(T register)
	{
		if (!registers.Contains(register))
		{
			registers.Add(register);

			onCollectionChanged?.Invoke();
		}
	}

	public virtual void UnRegistrate(T register)
	{
		if (registers.Contains(register))
		{
			registers.Remove(register);

			onCollectionChanged?.Invoke();
		}
	}


	public REGISTR GetAs<REGISTR>() where REGISTR : class, T
	{
		return Get<REGISTR>() as REGISTR;
	}

	public T Get<REGISTR>() where REGISTR : T
	{
		if (ContainsType<REGISTR>())
		{
			return registers.Where((window) => window is REGISTR).FirstOrDefault();
		}

		throw new System.Exception($"REGISTRATOR DOESN'T CONTAINS {typeof(REGISTR)} ERROR");
	}

	public bool ContainsType<REGISTR>() where REGISTR : T
	{
		return registers.OfType<REGISTR>().Any();
	}

	public bool ContainsType<REGISTR>(out REGISTR registr) where REGISTR : T
	{
		registr = registers.OfType<REGISTR>().FirstOrDefault();
		return registr != null;
	}
}