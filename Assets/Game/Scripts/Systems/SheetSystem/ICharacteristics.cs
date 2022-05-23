using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
	public interface ICharacteristics
	{
		ICharacteristic<int> Level { get; }
		ICharacteristic<int> Experience { get; }
		ICharacteristic<Vector2> Alignment { get; }
	}

	public class Characteristics : ICharacteristics
	{
		public ICharacteristic<int> Level { get; }
		public ICharacteristic<int> Experience { get; }
		public ICharacteristic<Vector2> Alignment { get; }

		public Characteristics(CharacteristicsSettings settings)
		{
			Level = new LevelCharacteristic(1);
			Experience = new ExperienceCharacteristic(0);
			Alignment = new AlignmentCharacteristic(settings.alignment);
		}

		public Characteristics(Data data)
		{
			Level = new LevelCharacteristic(data.level);
			Experience = new ExperienceCharacteristic(data.experiance);
			Alignment = new AlignmentCharacteristic(data.alignment);
		}

		public Data GetData()
		{
			return new Data()
			{
				level = Level.CurrentValue,
				experiance = Experience.CurrentValue,
				alignment = Alignment.CurrentValue,
			};
		}

		public class Data
		{
			public int level;
			public int experiance;
			public Vector2 alignment;
		}
	}


	[System.Serializable]
	public class CharacteristicsSettings
	{
		public AlignmentType alignment = AlignmentType.TrueNeutral;
	}
}