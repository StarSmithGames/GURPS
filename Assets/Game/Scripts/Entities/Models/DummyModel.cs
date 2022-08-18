using Game.Entities.Models;
using Game.Systems.DialogueSystem;
using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
	public class DummyModel : Model, ISheetable, IActor
	{
		public bool IsHaveSomethingToSay => actorSettings != null;
		public bool IsInDialogue { get; set; }

		public Transform DialogueTransform => transform;

		public ActorSettings ActorSettings => actorSettings;
		[SerializeField] private ActorSettings actorSettings;

		public ISheet Sheet
		{
			get
			{
				if (sheet == null)
				{
					sheet = new ModelSheet(data);
				}

				return sheet;
			}
		}

		private ModelSheet sheet;
		[SerializeField] private ModelData data;

		public void Bark()
		{

		}
	}
}