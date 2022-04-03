using Game.Systems.SheetSystem;

using UnityEngine;

using Zenject;

namespace Game.Entities
{
	public class NPC : Entity
	{
		[SerializeField] private NPCData data;

		public override ISheet Sheet
		{
			get
			{
				if (npcSheet == null)
				{
					npcSheet = new NPCSheet(data);
				}

				return npcSheet;
			}
		}
		private NPCSheet npcSheet;

		protected FieldOfView fov;

		[Inject]
		private void Construct(FieldOfView fov)
		{
			this.fov = fov;
		}
	}
}