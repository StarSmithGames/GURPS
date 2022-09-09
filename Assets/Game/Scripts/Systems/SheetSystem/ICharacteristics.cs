using Game.Systems.SheetSystem;

using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
	public interface ICharacteristics
	{
		ICharacteristic<int> Level { get; }
		ICharacteristic<int> Experience { get; }
		ICharacteristic<Vector2> Alignment { get; }
		ICharacteristic<Deity> Deity { get; }
		ICharacteristic<Handed> Handed { get; }
	}

	public class Characteristics : ICharacteristics
	{
		public ICharacteristic<int> Level { get; }
		public ICharacteristic<int> Experience { get; }
		public ICharacteristic<Vector2> Alignment { get; }
		public ICharacteristic<Deity> Deity { get; }
		public ICharacteristic<Handed> Handed { get; }


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

	[HideLabel]
	[System.Serializable]
	public class CharacteristicsSettings
	{
		[Title("ALignment", horizontalLine: false, bold: false)]
		[HideLabel]
		[EnumToggleButtons]
		[HorizontalGroup("Split", 300, MarginLeft = 0.25f, MarginRight = 0.25f)]
		public AlignmentType alignment = AlignmentType.TrueNeutral;
		public Deity deity = Deity.None;
		public Handed handed = Handed.Right;
	}

	public enum Handed
	{
		None,
		Right,
		Left,
		Ambidextrous,
	}

	public enum Deity
	{
		None,
		Cthulhu,
	}
}