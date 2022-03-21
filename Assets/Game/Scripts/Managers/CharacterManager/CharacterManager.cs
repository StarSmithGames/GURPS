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
		public Character CurrentCharacter { get; private set; }

		public int CurrentCharacterIndex => characters.IndexOf(CurrentCharacter);

		private List<Character> characters = new List<Character>();

		private SignalBus signalBus;

		public CharacterManager(SignalBus signalBus)
		{
			this.signalBus = signalBus;
		}

		public void Initialize()
		{
			characters = GameObject.FindObjectsOfType<Character>().ToList();//stub
			SetCharacter(characters.FirstOrDefault());
		}

		public void Dispose() { }

		public void SetCharacter(Character character)
		{
			if (CurrentCharacter != character)
			{
				CurrentCharacter = character;
				signalBus?.Fire(new SignalCharacterChanged() { character = CurrentCharacter });
			}
		}

		public void SetCharacter(int index)
		{
			SetCharacter(characters[index]);
		}
	}
}