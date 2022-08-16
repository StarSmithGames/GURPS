using Game.Entities;
using Game.Entities.Models;

using System;
using System.Collections.Generic;
using Zenject;

namespace Game.Managers.CharacterManager
{
	public class CharacterManager : IInitializable, IDisposable
	{
		public PlayerModel Player { get; private set; }
		public PlayerRTSModel PlayerRTS { get; private set; }

		private List<ICharacterModel> characters = new List<ICharacterModel>();
		private List<ICompanionModel> party = new List<ICompanionModel>();

		private SignalBus signalBus;
		private GameManager.GameManager gameManager;

		public CharacterManager(SignalBus signalBus, GameManager.GameManager gameManager)
		{
			this.signalBus = signalBus;
			this.gameManager = gameManager;
		}

		public void Initialize()
		{
			//CurrentParty = new CharacterParty(signalBus, GameObject.FindObjectsOfType<Character>().ToList());//stub
			//CurrentParty.SetLeader(0);
		}

		public void Dispose() { }

		#region Registration
		public void Registrate(PlayerModel player)
		{
			Player = player;
		}

		public void UnRegistrate(PlayerModel player)
		{
			Player = null;
		}

		public void Registrate(PlayerRTSModel player)
		{
			PlayerRTS = player;
		}

		public void UnRegistrate(PlayerRTSModel player)
		{
			PlayerRTS = null;
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
		#endregion
	}
}