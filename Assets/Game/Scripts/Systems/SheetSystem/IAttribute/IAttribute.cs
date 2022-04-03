using System.Collections.Generic;

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

		public override abstract string ToString();

		protected virtual void ValueChanged()
		{
			onAttributeChanged?.Invoke();
		}
	}

	public abstract class AttributeModifiable : Attribute, IModifiable
	{
		public List<IModifier> Modifiers { get; private set; }

		public AttributeModifiable()
		{
			Modifiers = new List<IModifier>();
		}

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

		public void AddModifier(IModifier modifier)
		{
			if (!Modifiers.Contains(modifier))
			{
				Modifiers.Add(modifier);
				ValueChanged();
			}
		}
		public void RemoveModifier(IModifier modifier)
		{
			if (Modifiers.Contains(modifier))
			{
				Modifiers.Remove(modifier);
				ValueChanged();
			}
		}
	}
}