using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Managers.GameManager
{
	public struct SignalGameStateChanged
	{
		public GameState newGameState;
		public GameState oldGameState;
	}
}