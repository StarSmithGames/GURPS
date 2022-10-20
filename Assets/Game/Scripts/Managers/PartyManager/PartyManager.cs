using Game.Entities;
using Game.Managers.StorageManager;
using System.Collections.Generic;
using Zenject;

namespace Game.Managers.PartyManager
{
    public class PartyManager
    {
		public PlayerParty PlayerParty { get; private set; }

		private PlayerParty.Factory playerFactory;
		private CharacterManager.CharacterManager characterManager;
		private ISaveLoad saveLoad;

		public PartyManager(PlayerParty.Factory playerFactory, CharacterManager.CharacterManager characterManager, ISaveLoad saveLoad)
		{
			this.playerFactory = playerFactory;
			this.characterManager = characterManager;
			this.saveLoad = saveLoad;
		}

		public void CreatePlayerParty()
		{
			PlayerParty = playerFactory.Create(characterManager.Player);
		}
	}

	public class Party
    {
		public List<ICharacter> Characters { get; private set; }

		protected SignalBus signalBus;

		public Party(SignalBus signalBus)
		{
			this.signalBus = signalBus;

			Characters = new List<ICharacter>();
		}

		public virtual void AddCharacter(ICharacter character)
		{
			if (!Characters.Contains(character))
			{
				Characters.Add(character);
				signalBus?.Fire(new SignalPartyChanged());
			}
		}

		public virtual void RemoveCharacter(ICharacter character)
		{
			if (Characters.Contains(character))
			{
				Characters.Remove(character);
				signalBus?.Fire(new SignalPartyChanged());
			}
		}

		public bool Contains(ICharacter character)
		{
			return Characters.Contains(character);
		}

		public class Data
		{
			
		}
	}

	public class PlayerParty : Party
	{
		public ICharacter LeaderParty { get; private set; }
		public int LeaderPartyIndex => Characters.IndexOf(LeaderParty);

		public PlayerParty(SignalBus signalBus, ICharacter player) : base(signalBus)
		{
			this.signalBus = signalBus;
			SetLeader(player);
			AddCharacter(player);
		}

		public bool SetLeader(ICharacter character)
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

		public class Factory : PlaceholderFactory<ICharacter, PlayerParty> { }
	}
}