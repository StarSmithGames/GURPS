using Game.Entities;
using Game.Entities.Models;

using System;
using System.Collections.Generic;
using Zenject;

namespace Game.Managers.CharacterManager
{
	public class CharacterManager
	{
		private List<ICharacterModel> characters = new List<ICharacterModel>();

		private SignalBus signalBus;
		private GameManager.GameManager gameManager;

		public CharacterManager(SignalBus signalBus, GameManager.GameManager gameManager)
		{
			this.signalBus = signalBus;
			this.gameManager = gameManager;
		}

		public void Registrate(ICharacterModel character)
		{
			if (!characters.Contains(character))
			{
				characters.Add(character);
			}
		}

		public void UnRegistrate(ICharacterModel character)
		{
			if (characters.Contains(character))
			{
				characters.Remove(character);
			}
		}
	}
}