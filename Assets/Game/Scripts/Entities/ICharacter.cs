using Game.Entities.Models;
using Game.Systems.DialogueSystem;
using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
	public interface ICharacter : IEntity, IActor { }

	public abstract class Character : Entity, ICharacter
	{
		public bool IsHaveSomethingToSay { get; }
		public bool IsInDialogue { get; set; }
		public ActorSettings ActorSettings { get; }
		public Transform DialogueTransform { get; }

		public void Bark()
		{
			throw new System.NotImplementedException();
		}
	}
}