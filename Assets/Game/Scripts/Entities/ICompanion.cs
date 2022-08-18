using Game.Entities.Models;
using Game.Managers.PartyManager;
using Game.Systems.SheetSystem;

using System;

using Zenject;

namespace Game.Entities
{
	public interface ICompanion : ICharacter
	{
		CompanionData Data { get; }
	}

	public class Companion : Character, ICompanion
	{
		public CompanionData Data { get; private set; }

		public override ISheet Sheet
		{
			get
			{
				if(sheet == null)
				{
					sheet = new CharacterSheet(Data);
				}

				return sheet;
			}
		}
		private ISheet sheet = null;

		public Companion(ICompanionModel model, CompanionData data)
		{
			Data = data;
			Model = model;
		}
	}
}