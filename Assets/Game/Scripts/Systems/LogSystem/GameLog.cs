using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.LogSystem
{
	public class GameLog
	{
		private DiContainer container;

		public GameLog(DiContainer container)
		{
			this.container = container;
		}

		public static void Push(string s)
		{

		}
	}
}