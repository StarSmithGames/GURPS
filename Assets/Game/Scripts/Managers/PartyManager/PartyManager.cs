using Game.Entities;

using System.Collections.Generic;
using System.Linq;

using UnityEngine.Assertions;

using Zenject;

namespace Game.Managers.PartyManager
{
    public class PartyManager : IInitializable
    {
		public PlayerParty PlayerParty
		{
			get
			{
				if(playerParty == null)
				{
					Assert.IsNotNull(signalBus, "PartyManager lost component");
					Assert.IsNotNull(player, "PartyManager lost component");

					playerParty = new PlayerParty(signalBus, player);
				}

				return playerParty;
			}
		}
		private PlayerParty playerParty = null;

		private SignalBus signalBus;
		private IPlayer player;

		public PartyManager(SignalBus signalBus, IPlayer player)
		{
			this.signalBus = signalBus;
			this.player = player;
		}

		public void Initialize()
		{
			playerParty = new PlayerParty(signalBus, player);
			playerParty.AddCharacter(player);
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
	}

	public class PlayerParty : Party
	{
		public ICharacter LeaderParty { get; private set; }
		public int LeaderPartyIndex => Characters.IndexOf(LeaderParty);

		public PlayerParty(SignalBus signalBus, IPlayer leader) : base(signalBus)
		{
			this.signalBus = signalBus;
			SetLeader(leader);
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


		public bool ContainsByData(CompanionData data)
		{
			return false;//Characters.Any((x) => (x is ICompanion) ? x. == data : false);
		}
	}
}