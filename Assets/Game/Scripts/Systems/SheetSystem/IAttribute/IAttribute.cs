using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace Game.Systems.SheetSystem
{
	public interface IAttribute
	{
		event UnityAction onAttributeChanged;
		string ToString();
	}

	public abstract class Attribute : IAttribute
	{
		public event UnityAction onAttributeChanged;

		protected virtual void ValueChanged()
		{
			onAttributeChanged?.Invoke();
		}
	}

	public abstract class AttributeFloatModifiable : Attribute, IModifiable<float>
	{
		public List<IModifier<float>> Modifiers { get; private set; }
		public float ModifyValue
		{
			get
			{
				float modifyValue = 0;
				for (int i = 0; i < Modifiers.Count; i++)
				{
					modifyValue += Modifiers[i].Value;
				}
				return modifyValue;
			}
		}

		public AttributeFloatModifiable()
		{
			Modifiers = new List<IModifier<float>>();
		}
		

		public void AddModifier(IModifier<float> modifier)
		{
			if (!Modifiers.Contains(modifier))
			{
				Modifiers.Add(modifier);
				ValueChanged();
			}
		}
		public void RemoveModifier(IModifier<float> modifier)
		{
			if (Modifiers.Contains(modifier))
			{
				Modifiers.Remove(modifier);
				ValueChanged();
			}
		}
	}

	public abstract class AttributeVecto2Modifiable : Attribute, IModifiable<Vector2>
	{
		public List<IModifier<Vector2>> Modifiers { get; private set; }
		public Vector2 ModifyValue
		{
			get
			{
				Vector2 modifyValue = Vector2.zero;
				for (int i = 0; i < Modifiers.Count; i++)
				{
					modifyValue += Modifiers[i].Value;
				}
				return modifyValue;
			}
		}

		public AttributeVecto2Modifiable()
		{
			Modifiers = new List<IModifier<Vector2>>();
		}


		public void AddModifier(IModifier<Vector2> modifier)
		{
			if (!Modifiers.Contains(modifier))
			{
				Modifiers.Add(modifier);
				ValueChanged();
			}
		}
		public void RemoveModifier(IModifier<Vector2> modifier)
		{
			if (Modifiers.Contains(modifier))
			{
				Modifiers.Remove(modifier);
				ValueChanged();
			}
		}
	}
}