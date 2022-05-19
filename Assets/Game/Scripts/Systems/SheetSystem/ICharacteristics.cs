using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
	public interface ICharacteristics
	{
		ICharacteristic<Vector2> Alignment { get; }
	}

	public class Characteristics : ICharacteristics
	{
		public ICharacteristic<Vector2> Alignment { get; }

		public Characteristics(CharacteristicsSettings settings)
		{
			Alignment = new AlignmentCharacteristic(settings.alignment);
		}

		public Data GetData()
		{
			return new Data()
			{
				alignment = Alignment.CurrentValue,
			};
		}

		public class Data
		{
			public Vector2 alignment;
		}
	}

	[System.Serializable]
	public class CharacteristicsSettings
	{
		public Alignment alignment = Alignment.TrueNeutral;
	}
}