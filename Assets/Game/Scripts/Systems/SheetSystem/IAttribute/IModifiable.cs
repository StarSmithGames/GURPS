using System.Collections.Generic;

namespace Game.Systems.SheetSystem
{
	public interface IModifiable
	{
		List<IModifier> Modifiers { get; }

		float ModifyValue { get; }

		void AddModifier(IModifier modifier);
		void RemoveModifier(IModifier modifier);
	}
}