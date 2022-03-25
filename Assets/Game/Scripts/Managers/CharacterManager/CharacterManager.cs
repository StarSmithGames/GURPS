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
		public CharacterParty Party { get; private set; }

		private SignalBus signalBus;

		public CharacterManager(SignalBus signalBus)
		{
			this.signalBus = signalBus;
		}

		public void Initialize()
		{
			Party = new CharacterParty(signalBus, GameObject.FindObjectsOfType<Character>().ToList());//stub
			Party.SetCharacter(0);
		}

		public void Dispose() { }
	}

	public class CharacterParty
	{
		public Character CurrentCharacter { get; private set; }
		public int CurrentCharacterIndex => Characters.IndexOf(CurrentCharacter);

		public List<Character> Characters { get; private set; }

		private SignalBus signalBus;

		public CharacterParty(SignalBus signalBus, List<Character> characters)
		{
			this.signalBus = signalBus;
			Characters = characters;
		}

		public bool SetCharacter(Character character)
		{
			if (CurrentCharacter != character)
			{
				CurrentCharacter = character;
				signalBus?.Fire(new SignalCharacterChanged() { character = CurrentCharacter });

				return true;
			}

			return false;
		}

		public bool SetCharacter(int index)
		{
			return SetCharacter(Characters[index]);
		}
	}
}