using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace Game.Systems.SheetSystem
{
	public interface ICharacteristic<T> : IAttribute where T : struct
	{
		event UnityAction onCharacteristicChanged;

		T CurrentValue { get; set; }
	}

	public interface ICharacteristicModifiable<T> : ICharacteristic<T> , IModifiable<T> where T : struct
	{

	}

	public abstract class Characteristic<T> : Attribute, ICharacteristic<T> where T : struct
	{
		public virtual T CurrentValue
		{
			get => currentValue;
			set
			{
				currentValue = value;

				ValueChanged();
			}
		}
		protected T currentValue;

		public event UnityAction onCharacteristicChanged;

		protected Characteristic(T currentValue)
		{
			CurrentValue = currentValue;
		}
	}

	public abstract class CharacteristicVector2Modifiable : AttributeVecto2Modifiable, ICharacteristicModifiable<Vector2>
	{
		public event UnityAction onCharacteristicChanged;

		public virtual Vector2 CurrentValue
		{
			get => currentValue;
			set
			{
				currentValue = value;

				ValueChanged();
			}
		}
		protected Vector2 currentValue;

		public CharacteristicVector2Modifiable(Vector2 currentValue) : base()
		{
			this.currentValue = currentValue;
		}

		protected override void ValueChanged()
		{
			base.ValueChanged();

			onCharacteristicChanged?.Invoke();
		}

		public override string ToString()
		{
			return CurrentValue.ToString();
		}
	}

	#region Characteristics
	public class ExperienceCharacteristic : Characteristic<int>
	{
		public ExperienceCharacteristic(int currentValue) : base(currentValue) { }
	}

	public class LevelCharacteristic : Characteristic<int>
	{
		public LevelCharacteristic(int currentValue) : base(currentValue) { }
	}

	public class AlignmentCharacteristic : Characteristic<Vector2>
	{
		public AlignmentType AlignmentType { get; private set; }
		public override Vector2 CurrentValue
		{
			get => base.CurrentValue;
			set
			{
				AlignmentType = Alignment.ConvertVector2ToAlignment(value);
				base.CurrentValue = value;
			}
		}

		public AlignmentCharacteristic(Vector2 currentValue) : base(currentValue)
		{
			AlignmentType = Alignment.ConvertVector2ToAlignment(currentValue);
		}
		public AlignmentCharacteristic(AlignmentType aligment) : base(Alignment.ConvertAlignmentToVector2(aligment))
		{
			AlignmentType = aligment;
		}
	}
	#endregion
}