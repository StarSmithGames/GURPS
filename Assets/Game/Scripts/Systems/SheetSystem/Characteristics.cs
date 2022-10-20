using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
	public sealed class Characteristics
	{
		public ICharacteristic<int> Level { get; }
		public ICharacteristic<float> Experience { get; }

		public ICharacteristic<string> DisplayName { get; }
		public ICharacteristic<Sprite> Portrait { get; }
		public ICharacteristic<Identity> Identity { get; }
		public ICharacteristic<RaceType> Race { get; }
		public ICharacteristic<GenderType> Gender { get; }
		public ICharacteristic<float> Age { get; }

		public ICharacteristic<Vector2> Alignment { get; }
		public ICharacteristic<Deity> Deity { get; }
		public ICharacteristic<Handed> Handed { get; }

		public Characteristics(CharacteristicsSettings settings)
		{
			Level = new LevelCharacteristic(1);
			Experience = new ExperienceCharacteristic(0f);

			DisplayName = new NameCharacteristic("");
			Portrait = new PortraitCharacteristic(null);
			Identity = new IdentityCharacteristic(settings.identity);
			Age = new AgeCharacteristic(settings.age);
			Race = new RaceCharacteristic(settings.race);
			Gender = new GenderCharacteristic(settings.gender);

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
			public float experiance;
			public Vector2 alignment;
		}
	}

	[HideLabel]
	[System.Serializable]
	public class CharacteristicsSettings
	{
		//Birthday

		public Identity identity = Identity.Humanoid;

		[HideIf("IsLifeless")]
		[Range(0, 100)]
		public float age = 0;
		
		[ShowIf("@IsHumanoid && !IsLifeless")]
		public RaceType race = RaceType.Human;
		[HideIf("IsLifeless")]
		public GenderType gender = GenderType.Male;
		

		[Title("ALignment", horizontalLine: false, bold: false)]
		[HideLabel]
		[EnumToggleButtons]
		[HorizontalGroup("Split", 300, MarginLeft = 0.25f, MarginRight = 0.25f)]
		public AlignmentType alignment = AlignmentType.TrueNeutral;
		public Deity deity = Deity.None;

		public Handed handed = Handed.Right;

		public bool IsHumanoid => identity == Identity.Humanoid;
		public bool IsLifeless => identity == Identity.Lifeless;
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

	public enum GenderType
	{
		Male,
		Female,
		Neuter,
		Neutral,
	}

	public enum Identity
	{
		Humanoid = 0,
		Animal = 10,
		Lifeless = 100,
	}
}