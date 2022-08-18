using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

namespace Game.Systems.CommandCenter
{
	public class CommandCenter : MonoBehaviour
	{
		public static CommandCenter Instance
		{
			get
			{
				if(instance == null)
				{
					instance = FindObjectOfType<CommandCenter>();
				}

				return instance;
			}
		}
		private static CommandCenter instance = null;

		public Registrator<IExecutor> Registrator
		{
			get
			{
				if(registrator == null)
				{
					registrator = new Registrator<IExecutor>();
				}

				return registrator;
			}
		}
		private Registrator<IExecutor> registrator;

		private DiContainer container;

		[Inject]
		private void Construct(DiContainer container)
		{
			this.container = container;
		}
	}
}

public class Registrator<T>
{
	private List<T> registers = new List<T>();

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
			return registers.Where((window) => window is T).FirstOrDefault();
		}

		throw new System.Exception($"REGISTRATOR DOESN'T CONTAINS {typeof(REGISTR)} ERROR");
	}

	public bool Contains<REGISTR>() where REGISTR : T
	{
		return registers.OfType<REGISTR>().Any();
	}
}