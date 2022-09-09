using Game.Entities.Models;
using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
	public interface ICharacter : IEntity, ISheetable { }

	public abstract class Character : Entity, ICharacter
	{
		public virtual ISheet Sheet { get; protected set; }

		public Character(ICharacterModel model, CharacterData data)
		{
			Sheet = new CharacterSheet(data);
			Model = model;
		}
	}

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
		public Transform Transform => null;//gameManager.CurrentGameLocation == GameLocation.Map ? RTSModel.transform : Model.Transform; 

		public PlayerRTSModel RTSModel { get; protected set; }

		public Player(ICharacterModel model, CharacterData data) : base(model, data) { }

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

	public class NPC : Character
	{
		public NPC(ICharacterModel model, CharacterData data) : base(model, data) { }
	}

	public interface ICompanion : ICharacter { }

	public class Companion : Character, ICompanion
	{
		public Companion(ICharacterModel model, CharacterData data) : base(model, data) { }
	}
}