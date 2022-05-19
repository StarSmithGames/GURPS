using System.Collections.Generic;

namespace Game.Systems.SheetSystem
{
	public interface IModifiable<T> where T : struct
	{
		List<IModifier<T>> Modifiers { get; }

		T ModifyValue { get; }

		void AddModifier(IModifier<T> modifier);
		void RemoveModifier(IModifier<T> modifier);
	}
}