using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems
{
	public static class GlobalRules { }

	public static class Alignment
	{

		/// <summary>
		/// LawfulGood(-1, 1)		NeutralGood(0, 1)		ChaoticGood(1, 1)
		/// LawfulNeutral(-1, 0)	TrueNeutral(0, 0)		ChaoticNeutral(1, 0)
		/// LawfulEvil(-1, -1)		NeutralEvil(0, -1)		ChaoticEvil(1, -1)
		/// </summary>
		public static List<Data> NineAlignments = new List<Data>()
		{
			new Data()
			{
				type = AlignmentType.LawfulGood,
				vector = new Vector2(-1, 1),
				color = Color.cyan,
			},
			new Data()
			{
				type = AlignmentType.NeutralGood,
				vector = new Vector2(0, 1),
				color = Color.cyan,
			},
			new Data()
			{
				type = AlignmentType.ChaoticGood,
				vector = new Vector2(1, 1),
				color = Color.cyan,
			},

			new Data()
			{
				type = AlignmentType.LawfulNeutral,
				vector = new Vector2(-1, 0),
				color = Color.white,
			},
			new Data()
			{
				type = AlignmentType.TrueNeutral,
				vector = new Vector2(0, 0),
				color = Color.white,
			},
			new Data()
			{
				type = AlignmentType.ChaoticNeutral,
				vector = new Vector2(1, 0),
				color = Color.white,
			},

			new Data()
			{
				type = AlignmentType.LawfulEvil,
				vector = new Vector2(-1, -1),
				color = Color.red,
			},
			new Data()
			{
				type = AlignmentType.NeutralEvil,
				vector = new Vector2(0, -1),
				color = Color.red,
			},
			new Data()
			{
				type = AlignmentType.ChaoticEvil,
				vector = new Vector2(1, -1),
				color = Color.red,
			},
		};


		public static AlignmentType ConvertVector2ToAlignment(Vector2 vector)
		{
			float minDistance = float.MaxValue;
			AlignmentType closetAlignment = AlignmentType.TrueNeutral;

			foreach (var a in NineAlignments)
			{
				float distance = Vector2.Distance(vector, a.vector);
				if (distance < minDistance)
				{
					minDistance = distance;
					closetAlignment = a.type;
				}
			}

			return closetAlignment;
		}

		public static Vector2 ConvertAlignmentToVector2(AlignmentType alignment)
		{
			return NineAlignments.Find((x) => x.type == alignment).vector;
		}

		public static Color GetAlignmentColor(AlignmentType alignment)
		{
			return NineAlignments.Find((x) => x.type == alignment).color;
		}


		public class Data
		{
			public AlignmentType type;
			public Vector2 vector;
			public Color color;
		}
	}


	//https://rpg.fandom.com/ru/wiki/%D0%9C%D0%B8%D1%80%D0%BE%D0%B2%D0%BE%D0%B7%D0%B7%D1%80%D0%B5%D0%BD%D0%B8%D0%B5
	public enum AlignmentType
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