using Game.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game.Managers.CharacterManager
{
	public class CharacterManager : IInitializable, IDisposable
	{
		public CharacterParty CurrentParty { get; private set; }

		private SignalBus signalBus;

		public CharacterManager(SignalBus signalBus)
		{
			this.signalBus = signalBus;
		}

		public void Initialize()
		{
			CurrentParty = new CharacterParty(signalBus, GameObject.FindObjectsOfType<Character>().ToList());//stub
			CurrentParty.SetLeader(0);
		}

		public void Dispose() { }
	}

	public class CharacterParty
	{
		public Character LeaderParty { get; private set; }
		public int LeaderPartyIndex => Characters.IndexOf(LeaderParty);

		public List<Character> Characters { get; private set; }

		private SignalBus signalBus;

		public CharacterParty(SignalBus signalBus, List<Character> characters)
		{
			this.signalBus = signalBus;
			Characters = characters;
			Characters.Shuffle();
		}

		public bool SetLeader(Character character)
		{
			if (LeaderParty != character)
			{
				LeaderParty = character;
				signalBus?.Fire(new SignalLeaderPartyChanged() { leader = LeaderParty });

				return true;
			}

			return false;
		}

		public bool SetLeader(int index)
		{
			if (index < Characters.Count)
			{
				return SetLeader(Characters[index]);
			}

			return false;
		}
	}
}