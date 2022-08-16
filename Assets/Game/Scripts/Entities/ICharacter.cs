using Game.Systems.DialogueSystem;
using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
	public interface ICharacter : IEntity, IActor
	{

	}

	public class Character : Entity, ICharacter
	{
		public override ISheet Sheet
		{
			get
			{
				if (characterSheet == null)
				{
					//characterSheet = new CharacterSheet(data);
				}

				return characterSheet;
			}
		}

		public bool IsHaveSomethingToSay { get; }
		public bool IsInDialogue { get; set; }
		public ActorSettings ActorSettings { get; }
		public Transform DialogueTransform { get; }

		private CharacterSheet characterSheet;

		public void Bark()
		{
			throw new System.NotImplementedException();
		}
	}
}