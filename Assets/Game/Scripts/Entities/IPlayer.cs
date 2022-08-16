using Game.Entities.Models;
using Game.Managers.CharacterManager;
using Game.Systems.SheetSystem;

namespace Game.Entities
{
	public interface IPlayer : ICharacter
	{
		public PlayerRTSModel RTSModel { get; }

		void Registrate(PlayerRTSModel player);
		void UnRegistrate(PlayerRTSModel player);

		void Registrate(PlayerModel player);
		void UnRegistrate(PlayerModel player);
	}

	public class Player : Character, IPlayer
	{
		public override ISheet Sheet => characterSheet;
		private CharacterSheet characterSheet;

		public PlayerRTSModel RTSModel { get; protected set; }
		public override IEntityModel Model { get; protected set; }

		public Player(CharacterDatabase database)
		{
			characterSheet = new CharacterSheet(database.player);
		}

		#region Registration
		public void Registrate(PlayerRTSModel player)
		{
			RTSModel = player;
		}

		public void UnRegistrate(PlayerRTSModel player)
		{
			RTSModel = null;
		}

		public void Registrate(PlayerModel player)
		{
			Model = player;
		}

		public void UnRegistrate(PlayerModel player)
		{
			Model = null;
		}
		#endregion
	}
}