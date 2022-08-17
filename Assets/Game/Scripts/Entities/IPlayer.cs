using Game.Entities.Models;
using Game.Managers.CharacterManager;
using Game.Managers.GameManager;
using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
	public interface IPlayer : ICharacter
	{
		public Transform Transform { get; }

		public PlayerRTSModel RTSModel { get; }

		void Registrate(PlayerRTSModel player);
		void UnRegistrate(PlayerRTSModel player);

		void Registrate(PlayerModel player);
		void UnRegistrate(PlayerModel player);
	}

	public class Player : Character, IPlayer
	{
		public Transform Transform => gameManager.CurrentGameLocation == GameLocation.Map ? RTSModel.transform : Model.Transform; 

		public PlayerRTSModel RTSModel { get; protected set; }

		private GameManager gameManager;

		public Player(CharacterDatabase database, GameManager gameManager)
		{
			this.gameManager = gameManager;
			Sheet = new CharacterSheet(database.player);
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