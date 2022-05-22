using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using static UnityEditor.Searcher.SearcherWindow;

namespace Game.Systems.SheetSystem
{
	public interface ICharacteristic<T> : IAttribute, IModifiable<T> where T : struct
	{
		event UnityAction onCharacteristicChanged;

		T CurrentValue { get; set; }
	}

	public abstract class CharacteristicVector2 : AttributeVecto2Modifiable, ICharacteristic<Vector2>
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

		public CharacteristicVector2(Vector2 currentValue) : base()
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

	public class AlignmentCharacteristic : CharacteristicVector2
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
}