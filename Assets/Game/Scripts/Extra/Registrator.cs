using System.Collections.Generic;
using System.Linq;

public class Registrator<T>
{
	public List<T> registers;

	public Registrator()
	{
		registers = new List<T>();
	}


	public void Registrate(T executor)
	{
		if (!registers.Contains(executor))
		{
			registers.Add(executor);
		}
	}

	public void UnRegistrate(T register)
	{
		if (registers.Contains(register))
		{
			registers.Remove(register);
		}
	}

	public REGISTR GetAs<REGISTR>() where REGISTR : class, T
	{
		return Get<REGISTR>() as REGISTR;
	}

	public T Get<REGISTR>() where REGISTR : T
	{
		if (Contains<REGISTR>())
		{
			return registers.Where((window) => window is REGISTR).FirstOrDefault();
		}

		throw new System.Exception($"REGISTRATOR DOESN'T CONTAINS {typeof(REGISTR)} ERROR");
	}

	public bool Contains<REGISTR>() where REGISTR : T
	{
		return registers.OfType<REGISTR>().Any();
	}
}