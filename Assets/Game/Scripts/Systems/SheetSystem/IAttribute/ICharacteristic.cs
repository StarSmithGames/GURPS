using UnityEngine;
using UnityEngine.Events;

namespace Game.Systems.SheetSystem
{
	public interface ICharacteristic<T> : IValue<T>
	{
		string Output { get; }

		string LocalizationKey { get; }
	}


	#region Characteristics
	public class LevelCharacteristic : ICharacteristic<int>
	{
		public event UnityAction onChanged;

		public string Output => $"Level {CurrentValue}";
		public string LocalizationKey => "sheet.characteristics.level";

		public int CurrentValue
		{
			get => currentValue;
			set
			{
				currentValue = value;
				onChanged?.Invoke();
			}
		}
		private int currentValue;

		public LevelCharacteristic(int value)
		{
			currentValue = value;
		}
	}

	public class ExperienceCharacteristic : ICharacteristic<float>
	{
		public event UnityAction onChanged;

		public float CurrentValue { get; set; }

		public string LocalizationKey { get; }
		public string Output { get; }

		public ExperienceCharacteristic(float currentValue)
		{
			CurrentValue = currentValue;
		}
	}

	public class NameCharacteristic : ICharacteristic<string>
	{
		public event UnityAction onChanged;

		public string Output => CurrentValue;
		public string LocalizationKey => "sheet.characteristics.name";

		public string CurrentValue
		{
			get => currentValue;
			set
			{
				currentValue = value;
				onChanged?.Invoke();
			}
		}
		private string currentValue;

		public NameCharacteristic(string value)
		{
			currentValue = value;
		}
	}

	public class PortraitCharacteristic : ICharacteristic<Sprite>
	{
		public event UnityAction onChanged;

		public string Output => "";
		public string LocalizationKey => "sheet.characteristics.portrait";

		public Sprite CurrentValue
		{
			get => sprite;
			set
			{
				sprite = value;
				onChanged?.Invoke();
			}
		}
		private Sprite sprite;

		public PortraitCharacteristic(Sprite value)
		{
			sprite = value;
		}
	}

	public class IdentityCharacteristic : ICharacteristic<Identity>
	{
		public event UnityAction onChanged;

		public string Output => CurrentValue.ToString();
		public string LocalizationKey => "sheet.characteristics.level";

		public Identity CurrentValue
		{
			get => currentValue;
			set
			{
				currentValue = value;
				onChanged?.Invoke();
			}
		}
		private Identity currentValue;

		public IdentityCharacteristic(Identity value)
		{
			currentValue = value;
		}
	}

	public class AgeCharacteristic : ICharacteristic<float>
	{
		public event UnityAction onChanged;

		public string Output => CurrentValue.ToString();
		public string LocalizationKey => "sheet.characteristics.level";

		public float CurrentValue
		{
			get => currentValue;
			set
			{
				currentValue = value;
				onChanged?.Invoke();
			}
		}
		private float currentValue;

		public AgeCharacteristic(float value)
		{
			currentValue = value;
		}
	}

	public class RaceCharacteristic : ICharacteristic<RaceType>
	{
		public event UnityAction onChanged;

		public string Output => CurrentValue.ToString();
		public string LocalizationKey => "sheet.characteristics.race";

		public RaceType CurrentValue
		{
			get => currentValue;
			set
			{
				currentValue = value;
				onChanged?.Invoke();
			}
		}
		private RaceType currentValue;

		public RaceCharacteristic(RaceType race)
		{
			currentValue = race;
		}
	}

	public class GenderCharacteristic : ICharacteristic<GenderType>
	{
		public event UnityAction onChanged;

		public string Output => CurrentValue.ToString();
		public string LocalizationKey => "sheet.characteristics.gender";

		public GenderType CurrentValue
		{
			get => currentValue;
			set
			{
				currentValue = value;
				onChanged?.Invoke();
			}
		}
		private GenderType currentValue;

		public GenderCharacteristic(GenderType gender)
		{
			currentValue = gender;
		}
	}

	public class AlignmentCharacteristic : ICharacteristic<Vector2>
	{
		public event UnityAction onChanged;

		public AlignmentType AlignmentType { get; private set; }
		public Vector2 CurrentValue
		{
			get => currentValue;
			set
			{
				AlignmentType = Alignment.ConvertVector2ToAlignment(value);
				currentValue = value;
			}
		}
		private Vector2 currentValue;

		public string LocalizationKey { get; }
		
		public string Output { get; }

		public AlignmentCharacteristic(Vector2 currentValue)
		{
			AlignmentType = Alignment.ConvertVector2ToAlignment(currentValue);
		}
		public AlignmentCharacteristic(AlignmentType aligment)
		{
			AlignmentType = aligment;
			currentValue = Alignment.ConvertAlignmentToVector2(aligment);
		}
	}
	#endregion
}