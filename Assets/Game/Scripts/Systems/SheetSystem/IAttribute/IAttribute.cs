using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace Game.Systems.SheetSystem
{
	public interface IAttribute : IValue<float>, IModifiable<AttributeModifier>
	{
		string Output { get; }

		string LocalizationKey { get; }
	}

	#region Attribute
	public abstract partial class Attribute : IAttribute
	{
		public event UnityAction onChanged;

		public virtual string Output => TotalValue.ToString();

		public virtual string LocalizationKey => "sheet.";

		public virtual float CurrentValue
		{
			get => currentValue;
			set
			{
				currentValue = value;
				onChanged?.Invoke();
			}
		}
		protected float currentValue;

		public Attribute(float currentValue)
		{
			this.currentValue = currentValue;

			Modifiers = new List<AttributeModifier>();
		}
	}

	//IModifiable Implementation
	public abstract partial class Attribute
	{
		public event UnityAction onModifiersChanged;

		public virtual float TotalValue => (CurrentValue + ModifyAddValue) * (1 + ModifyPercentValue);

		public virtual float ModifyAddValue
		{
			get
			{
				float value = 0;

				Modifiers.ForEach((modifier) =>
				{
					if (modifier is AddModifier)
					{
						value += modifier.CurrentValue;
					}
				});

				return value;
			}
		}

		public virtual float ModifyPercentValue
		{
			get
			{
				float value = 0;

				Modifiers.ForEach((modifier) =>
				{
					if (modifier is PercentModifier)
					{
						value += modifier.CurrentValue;
					}
				});

				return value;
			}
		}

		public List<AttributeModifier> Modifiers { get; }

		public virtual bool AddModifier(AttributeModifier modifier)
		{
			if (!Contains(modifier))
			{
				Modifiers.Add(modifier);

				onModifiersChanged?.Invoke();

				return true;
			}

			return false;
		}

		public virtual bool RemoveModifier(AttributeModifier modifier)
		{
			if (Contains(modifier))
			{
				Modifiers.Remove(modifier);

				onModifiersChanged?.Invoke();

				return true;
			}

			return false;
		}

		public bool Contains(AttributeModifier modifier) => Modifiers.Contains(modifier);
	}
	#endregion

	#region AttributeBar
	public abstract partial class AttributeBar : Attribute, IBar
	{
		public override string Output => $"{CurrentValue} / {TotalValue}";

		public override float CurrentValue
		{
			get => currentValue;
			set
			{
				base.CurrentValue = Mathf.Clamp(value, MinValue, TotalValue);
			}
		}

		public virtual float MaxValue
		{
			get => maxValue;
			set
			{
				maxValue = value;
				base.CurrentValue = Mathf.Clamp(currentValue, MinValue, TotalValue);
			}
		}
		protected float maxValue;

		public virtual float MinValue { get; protected set; }

		public float PercentValue => CurrentValue / TotalValue;

		protected AttributeBar(float value, float min, float max) : base(value)
		{
			this.maxValue = max;
			this.MinValue = min;
			this.CurrentValue = value;
		}
	}

	//IModifiable Implementation
	public abstract partial class AttributeBar
	{
		public override float TotalValue => (MaxValue + ModifyAddValue) * (1 + ModifyPercentValue);

		public override bool AddModifier(AttributeModifier modifier)
		{
			if (base.AddModifier(modifier))
			{
				CurrentValue = currentValue;//upd
				return true;
			}

			return false;
		}

		public override bool RemoveModifier(AttributeModifier modifier)
		{
			if (base.RemoveModifier(modifier))
			{
				CurrentValue = currentValue;//upd
				return true;
			}

			return false;
		}
	}
	#endregion
}