using Game.Entities;
using Game.Entities.Models;

namespace Game.Managers.CharacterManager
{
	public struct SignalLeaderPartyChanged
	{
		public CharacterModel leader;
	}
}