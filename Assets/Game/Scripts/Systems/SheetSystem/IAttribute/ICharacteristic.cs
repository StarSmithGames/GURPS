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
		public Alignment Alignment { get; private set; }
		public override Vector2 CurrentValue
		{
			get => base.CurrentValue;
			set
			{
				Alignment = ConvertVector2ToAlignment(value);
				base.CurrentValue = value;
			}
		}

		/// <summary>
		/// LawfulGood(-1, 1)		NeutralGood(0, 1)		ChaoticGood(1, 1)
		/// LawfulNeutral(-1, 0)	TrueNeutral(0, 0)		ChaoticNeutral(1, 0)
		/// LawfulEvil(-1, -1)		NeutralEvil(0, -1)		ChaoticEvil(1, -1)
		/// </summary>
		public static List<AlignmentData> NineAlignments = new List<AlignmentData>()
		{
			new AlignmentData()
			{
				type = Alignment.LawfulGood,
				vector = new Vector2(-1, 1),
				color = Color.cyan,
			},
			new AlignmentData()
			{
				type = Alignment.NeutralGood,
				vector = new Vector2(0, 1),
				color = Color.cyan,
			},
			new AlignmentData()
			{
				type = Alignment.ChaoticGood,
				vector = new Vector2(1, 1),
				color = Color.cyan,
			},

			new AlignmentData()
			{
				type = Alignment.LawfulNeutral,
				vector = new Vector2(-1, 0),
				color = Color.white,
			},
			new AlignmentData()
			{
				type = Alignment.TrueNeutral,
				vector = new Vector2(0, 0),
				color = Color.white,
			},
			new AlignmentData()
			{
				type = Alignment.ChaoticNeutral,
				vector = new Vector2(1, 0),
				color = Color.white,
			},

			new AlignmentData()
			{
				type = Alignment.LawfulEvil,
				vector = new Vector2(-1, -1),
				color = Color.red,
			},
			new AlignmentData()
			{
				type = Alignment.NeutralEvil,
				vector = new Vector2(0, -1),
				color = Color.red,
			},
			new AlignmentData()
			{
				type = Alignment.ChaoticEvil,
				vector = new Vector2(1, -1),
				color = Color.red,
			},
		};


		public AlignmentCharacteristic(Vector2 currentValue) : base(currentValue)
		{
			Alignment = ConvertVector2ToAlignment(currentValue);
		}
		public AlignmentCharacteristic(Alignment aligment) : base(ConvertAlignmentToVector2(aligment))
		{
			Alignment = aligment;
		}


		public static Alignment ConvertVector2ToAlignment(Vector2 vector)
		{
			float minDistance = float.MaxValue;
			Alignment closetAlignment = Alignment.TrueNeutral;

			foreach (var a in NineAlignments)
			{
				float distance = Vector2.Distance(vector, a.vector);
				if(distance < minDistance)
				{
					minDistance = distance;
					closetAlignment = a.type;
				}
			}

			return closetAlignment;
		}

		public static Vector2 ConvertAlignmentToVector2(Alignment alignment)
		{
			return NineAlignments.Find((x) => x.type == alignment).vector;
		}

		public static Color GetAlignmentColor(Alignment alignment)
		{
			return NineAlignments.Find((x) => x.type == alignment).color;
		}
	}

	public class AlignmentData
	{
		public Alignment type;
		public Vector2 vector;
		public Color color;
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