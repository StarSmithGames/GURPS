using Game.Entities;
using Game.Entities.Models;

using System.Collections.Generic;

using Zenject;

namespace Game.Managers.PartyManager
{
    public class PartyManager : IInitializable
    {
		public PlayerParty PlayerParty { get; private set; }

		private SignalBus signalBus;
		private IPlayer player;

		public PartyManager(SignalBus signalBus, IPlayer player)
		{
			this.signalBus = signalBus;
			this.player = player;
		}

		public void Initialize()
		{
			PlayerParty = new PlayerParty(signalBus, player);
		}

		//public void Registrate(ICompanion companion)
		//{
		//	if (!PlayerParty.Contains(companion))
		//	{
		//		PlayerParty.Add(companion);
		//	}
		//}

		//public void UnRegistrate(ICompanion companion)
		//{
			//if (party.Contains(companion))
			//{
			//	party.Remove(companion);
			//}
		//}
	}

	public class Party
    {
		public List<ICharacter> Characters { get; private set; }

		public Party()
		{
			Characters = new List<ICharacter>();
		}

		public virtual void AddCharacter(ICharacter character)
		{
			if (!Characters.Contains(character))
			{
				Characters.Add(character);
			}
		}

		public virtual void RemoveCharacter(ICharacter character)
		{
			if (Characters.Contains(character))
			{
				Characters.Remove(character);
			}
		}
	}

	public class PlayerParty : Party
	{
		public ICharacter LeaderParty { get; private set; }
		public int LeaderPartyIndex => Characters.IndexOf(LeaderParty);

		private SignalBus signalBus;

		public PlayerParty(SignalBus signalBus, IPlayer leader) : base()
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
	}
}