using Game.Systems.DialogueSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities
{
	public class DummyModel : InteractableModel, ISheetable, IActor
	{
		public bool IsHaveSomethingToSay => actorSettings != null;
		
		public Transform Transform => transform;

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