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
		public Alignment Aligment { get; private set; }
		public override Vector2 CurrentValue
		{
			get => base.CurrentValue;
			set
			{
				Aligment = ConvertVector2ToAligment(value);
				base.CurrentValue = value;
			}
		}

		public AlignmentCharacteristic(Vector2 currentValue) : base(currentValue)
		{
			Aligment = ConvertVector2ToAligment(currentValue);
		}
		public AlignmentCharacteristic(Alignment aligment) : base(ConvertAligmentToVector2(aligment))
		{
			Aligment = aligment;
		}


		private static Alignment ConvertVector2ToAligment(Vector2 vector)
		{
			return Alignment.TrueNeutral;
		}
		private static Vector2 ConvertAligmentToVector2(Alignment aligment)
		{
			switch (aligment)
			{
				case Alignment.NeutralGood:
				{
					return new Vector2(-1, 0);
				}
				case Alignment.TrueNeutral:
				{
					return new Vector2(0, 0);
				}
				case Alignment.NeutralEvil:
				{
					return new Vector2(1, 0);
				}

				case Alignment.LawfulNeutral:
				{
					return new Vector2(0, 1);
				}
				case Alignment.ChaoticNeutral:
				{
					return new Vector2(0, -1);
				}

				case Alignment.LawfulGood:
				{
					return new Vector2(-1, -1);
				}
				case Alignment.LawfulEvil:
				{
					return new Vector2(1, -1);
				}
				case Alignment.ChaoticEvil:
				{
					return new Vector2(1, 1);
				}
				case Alignment.ChaoticGood:
				{
					return new Vector2(-1, -1);
				}
			}

			return Vector2.zero;
		}
	}


	//https://rpg.fandom.com/ru/wiki/%D0%9C%D0%B8%D1%80%D0%BE%D0%B2%D0%BE%D0%B7%D0%B7%D1%80%D0%B5%D0%BD%D0%B8%D0%B5
	public enum Alignment
	{
		LawfulGood,
		NeutralGood,
		ChaoticGood,
		LawfulNeutral,
		TrueNeutral,
		ChaoticNeutral,
		LawfulEvil,
		NeutralEvil,
		ChaoticEvil,
	}
}