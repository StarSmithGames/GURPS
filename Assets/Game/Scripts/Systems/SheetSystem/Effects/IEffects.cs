using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.SheetSystem
{
	public interface IEffects
	{
		List<IEffect> Effects { get; }
	}

	public interface IEffect
	{
	}
}