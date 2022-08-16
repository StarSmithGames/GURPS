using Game.Entities;
using Game.Entities.Models;
using Game.Managers.CharacterManager;

using System.Collections.Generic;

using Zenject;

namespace Game.Managers.PartyManager
{
    public class PartyManager : IInitializable
    {
		public PlayerParty PlayerParty { get; private set; }

		public void Initialize()
		{
			PlayerParty = new PlayerParty();
		}

		public void Registrate(ICompanionModel companion)
		{
			//if (!party.Contains(companion))
			//{
			//	party.Add(companion);
			//}
		}

		public void UnRegistrate(ICompanionModel companion)
		{
			//if (party.Contains(companion))
			//{
			//	party.Remove(companion);
			//}
		}

		//public List<Character.Data> companions = new List<Character.Data>();
	}

	public class Party
    {
		public List<CharacterModel> Characters { get; private set; }

		//protected SignalBus signalBus;
	}
	public class PlayerParty : Party
	{
		public CharacterModel LeaderParty { get; private set; }
		public int LeaderPartyIndex => Characters.IndexOf(LeaderParty);

		public PlayerParty() : base()
		{

		}

		public bool SetLeader(CharacterModel character)
		{
			if (LeaderParty != character)
			{
				LeaderParty = character;
				//signalBus?.Fire(new SignalLeaderPartyChanged() { leader = LeaderParty });

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